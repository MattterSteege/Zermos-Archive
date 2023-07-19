using System.Linq;

namespace Zermos_Web.Utilities
{
    public static class stringUtils
    {
        /// <summary>
        /// Capitalizes the first letter of a string and returns the string.
        /// </summary>
        /// <param name="str">The string to capitalize. I.E. lorem</param>
        /// <returns>The capitalized string. I.E. Lorem</returns>
        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        
        /// <summary>
        /// Returns a string with the first x words of the string.
        /// </summary>
        /// <param name="str">The string to get the first x words of. I.E. Lorem ipsum dolor sit amet</param>
        /// <param name="wordCount">The amount of words to get. I.E. 3</param>
        /// <returns>The string with the first x words. I.E. Lorem ipsum dolor</returns>
        public static string FirstWords(this string str, int wordCount)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            var words = str.Split(' ');
            string returna = string.Join(" ", words.Take(wordCount));
            return returna;
        }
    }
}