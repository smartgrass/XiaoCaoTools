using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace GG.Extensions
{
    /// <summary>
    ///     Security Behaviour e.g. Hashing and the like
    /// </summary>
    public static class SecurityExtensions
    {
        const int Iterations = 10000;
        
        /// <summary>
        /// Convert a byte array into string
        /// </summary>
        /// <param name="_in"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] _in)
        {
            string s = string.Empty;
            foreach (byte b in _in)
            {
                s += (char) b;
            }

            return s;
        }
                        
        /// <summary>
        /// convert a string into bytes
        /// </summary>
        /// <param name="_in"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string _in)
        {
            List<byte> b = new List<byte>();
            foreach (char c in _in)
            {
                b.Add((byte) c);
            }

            return b.ToArray();
        }
        
        /// <summary>
        ///     Get the SHA256 hash of the input
        /// </summary>
        /// <param name="input">Input String</param>
        /// <returns>The SHA256 hash</returns>
        public static byte[] GetHash(this string input)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        ///     Get the input string in a SHA256 hash format
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>The hashed version of the string</returns>
        public static string GetHashString(this string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(input))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

#region Password Hash

        /// <summary>
        /// Encrypt the string using a randomly generated salt key
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string PasswordHashEncrypt(this string toEncrypt)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(toEncrypt, salt, Iterations);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            
            Array.Copy(salt,0,hashBytes, 0,16);
            Array.Copy(hash,0,hashBytes, 16,20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Encrypt the data by the salt key of the source and compare the encrypted data
        /// </summary>
        /// <param name="password"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        public static bool PasswordHashCompare(this string password, string original)
        {
            string savedPassword = original;
            byte[] hashBytes = Convert.FromBase64String(savedPassword);
                    
            byte[] salt = new byte[16];
            Array.Copy(hashBytes,0,salt,0,16);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(20);

            bool b = true;
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    b = false;
                }
            }

            return b;
        }

#endregion

#region Salt Key Encryption

        public static List<string> SaltKeyEncrypt(string data, string salt)
        {
            byte[] key = Encoding.UTF8.GetBytes(salt);
            using (HMACSHA512 sha512 = new HMACSHA512(key))
            {
                byte[] payload = Encoding.UTF8.GetBytes(data);
                byte[] binaryHash = sha512.ComputeHash(payload);
                string stringHash = Convert.ToBase64String(binaryHash);

                List<string> l = new List<string>
                {
                    Convert.ToBase64String(payload),
                    stringHash
                };

                return l;
            }
        }
        
        public static string SaltKeyDecrypt(string[] data, string salt)
        {
            string hash = data[1];
            byte[] key = Encoding.UTF8.GetBytes(salt);
            using (HMACSHA512 sha512 = new HMACSHA512(key))
            {
                byte[] payload = Convert.FromBase64String(data[0]);
                byte[] binaryHash = sha512.ComputeHash(payload);
                string stringHash = Convert.ToBase64String(binaryHash);

                if (hash == stringHash)
                {
                    string d = Encoding.UTF8.GetString(payload);
                    return d;
                }
            }

            return "";
        }

#endregion

#region AES
        public static string AesEncryption(string rawValue, string key, string iv)
        {
            byte[] keyByte = StringToBytes(key);
            byte[] ivByte = StringToBytes(iv);
            
            byte[] encrypted;
            string s = string.Empty;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyByte;
                aesAlg.IV = ivByte;
                
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(rawValue);
                        }

                        encrypted = msEncrypt.ToArray();

                        s = BytesToString(encrypted);
                    }
                }
            }

            return s;
        }

        /// <summary>
        ///  Decrypt the byte array into a ascii readable string
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AesDecryption(byte[] rawValue, string key, string iv)
        {
            byte[] keyByte = StringToBytes(key);
            byte[] ivByte = StringToBytes(iv);
            
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyByte;
                aesAlg.IV = ivByte;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(rawValue))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
#endregion
    }
}
