using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GG.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Given a person's first and last name, we'll make our best guess to extract up to two initials, hopefully
        /// representing their first and last name, skipping any middle initials, Jr/Sr/III suffixes, etc. The letters 
        /// will be returned together in ALL CAPS, e.g. "TW". 
        /// 
        /// The way it parses names for many common styles:
        /// 
        /// Mason Zhwiti                -> MZ
        /// mason lowercase zhwiti      -> MZ
        /// Mason G Zhwiti              -> MZ
        /// Mason G. Zhwiti             -> MZ
        /// John Queue Public           -> JP
        /// John Q. Public, Jr.         -> JP
        /// John Q Public Jr.           -> JP
        /// Thurston Howell III         -> TH
        /// Thurston Howell, III        -> TH
        /// Malcolm X                   -> MX
        /// A Ron                       -> AR
        /// A A Ron                     -> AR
        /// Madonna                     -> M
        /// Chris O'Donnell             -> CO
        /// Malcolm McDowell            -> MM
        /// Robert "Rocky" Balboa, Sr.  -> RB
        /// 1Bobby 2Tables              -> BT
        /// Éric Ígor                   -> ÉÍ
        /// 행운의 복숭아                 -> 행복
        /// 
        /// </summary>
        /// <param name="name">The full name of a person.</param>
        /// <returns>One to two uppercase initials, without punctuation.</returns>
        public static string ExtractInitialsFromName(string name)
        {
            // first remove all: punctuation, separator chars, control chars, and numbers (unicode style regexes)
            string initials = Regex.Replace(name, @"[\p{P}\p{S}\p{C}\p{N}]+", "");

            // Replacing all possible whitespace/separator characters (unicode style), with a single, regular ascii space.
            initials = Regex.Replace(initials, @"\p{Z}+", " ");

            // Remove all Sr, Jr, I, II, III, IV, V, VI, VII, VIII, IX at the end of names
            initials = Regex.Replace(initials.Trim(), @"\s+(?:[JS]R|I{1,3}|I[VX]|VI{0,3})$", "", RegexOptions.IgnoreCase);

            // Extract up to 2 initials from the remaining cleaned name.
            initials = Regex.Replace(initials, @"^(\p{L})[^\s]*(?:\s+(?:\p{L}+\s+(?=\p{L}))?(?:(\p{L})\p{L}*)?)?$", "$1$2").Trim();

            if (initials.Length > 2)
            {
                // Worst case scenario, everything failed, just grab the first two letters of what we have left.
                initials = initials.Substring(0, 2);
            }

            return initials.ToUpperInvariant();
        }

        public static string FirstNameAndInitial(string name)
        {
            name = Regex.Match(name, @"[A-Za-z]+\s+[A-Za-z]").Value;

            return name;
        }
        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string TitleCase(this string str)
        {
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
 
        /// <summary>
        /// Overload which uses the culture info with the specified name
        /// </summary>
        public static string TitleCase(this string str, string cultureInfoName)
        {
            CultureInfo cultureInfo = new CultureInfo(cultureInfoName);
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
 
        /// <summary>
        /// Overload which uses the specified culture info
        /// </summary>
        public static string TitleCase(this string str, CultureInfo cultureInfo)
        {
            return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
        }
        
        
        public static string[] SplitAtIndexs(this string source, params int[] index)
        {
            var indices = new[] {0}.Union(index).Union(new[] {source.Length});

            return indices
                .Zip(indices.Skip(1), (a, b) => (a, b))
                .Select(_ => source.Substring(_.a, _.b - _.a)).ToArray();
        }

        public static string SliceString(this string source, int maxCharacterLengthInLine)
        {
            if (source.Length >= maxCharacterLengthInLine)
            {
                //get half way number
                int half = source.Length / 2;
                bool startOfWord = false;

                while (!startOfWord)
                {
                    //while not space, go back a letter
                    char c = source[half];
                    if (c == ' ' || half == 0)
                    {
                        startOfWord = true;
                    }
                    else
                    {
                        half++;
                    }
                }

                string[] a = source.SplitAtIndexs(half);
                source = a[0] + "\n" + a[1].TrimStart();
            }

            return source;
        }
    }
}
