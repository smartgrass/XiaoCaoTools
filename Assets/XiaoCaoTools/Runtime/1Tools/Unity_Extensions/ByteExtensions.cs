using System;
 
namespace GG.Extensions
{
    /// <summary>
    ///     Byte Extensions class
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        ///     Is a bit set?
        /// </summary>
        /// <param name="b">The byte to compare</param>
        /// <param name="pos">The bit position to check</param>
        /// <returns>True if bit is set</returns>
        public static bool IsBitSet(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException(nameof(pos), "Index must be in the range of 0-7.");
 
            return (b & (1 << pos)) != 0;
        }
 
        /// <summary>
        ///     Set the bit (it's unclear if this works properly so i've not been using it currently).
        /// </summary>
        /// <param name="b">The byte to change</param>
        /// <param name="pos">The bit position to set</param>
        /// <returns>The new byte</returns>
        public static byte SetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException(nameof(pos), "Index must be in the range of 0-7.");
 
            return (byte)(b | (1 << pos));
        }
 
        /// <summary>
        ///     Unset a bit
        /// </summary>
        /// <param name="b">The byte to change</param>
        /// <param name="pos">The bit position to set</param>
        /// <returns>The new byte</returns>
        public static byte UnsetBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException(nameof(pos), "Index must be in the range of 0-7.");
 
            return (byte)(b & ~(1 << pos));
        }
 
        /// <summary>
        ///     Toggles a bit
        /// </summary>
        /// <param name="b">The byte to toggle</param>
        /// <param name="pos">The positon to toggle</param>
        /// <returns>The new byte</returns>
        public static byte ToggleBit(this byte b, int pos)
        {
            if (pos < 0 || pos > 7)
                throw new ArgumentOutOfRangeException(nameof(pos), "Index must be in the range of 0-7.");
 
            return (byte)(b ^ (1 << pos));
        }
 
        /// <summary>
        ///     Converts to a binary string
        /// </summary>
        /// <param name="b">The byte to compare</param>
        /// <returns>Byte in string format</returns>
        public static string ToBinaryString(this byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }
    }
}