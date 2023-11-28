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
            var returna = string.Join(" ", words.Take(wordCount));
            return returna;
        }
        
        /// <summary>
        /// Returns a string with the first x characters of the string.
        /// </summary>
        /// <param name="str">The string to get the first x characters of. I.E. Lorem ipsum dolor sit amet</param>
        /// <param name="charCount">The amount of characters to get. I.E. 10</param>
        /// <param name="addDots">If true, adds dots to the end of the string. I.E. Lorem ipsu..., but only if the string is longer than the amount of characters.</param>
        /// <returns>The string with the first x characters. I.E. Lorem ipsu</returns>
        public static string FirstChars(this string str, int charCount, bool addDots = false)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= charCount)
                return str;
            var returna = str.Substring(0, charCount);
            if (addDots)
                returna += "...";
            return returna;
        }

        /// <summary>
        /// Returns true if the string is null or empty.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            bool a = string.IsNullOrEmpty(str);
            return a;
        }
        
        /// <summary>
        /// Returns a base64 string of the object.
        /// </summary>
        /// <param name="obj">The object to convert to base64.</param>
        /// <returns>The base64 string of the object.</returns>
        public static string ObjectToBase64String(this object obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        }
        
        /// <summary>
        /// Returns an object of type T from a base64 string.
        /// </summary>
        /// <param name="base64">The base64 string to convert to an object.</param>
        /// <typeparam name="T">The type of the object to return.</typeparam>
        /// <returns>The object of type T from the base64 string.</returns>
        public static T Base64StringToObject<T>(this string base64)
        {
            string json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64));
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        
        public static string IntToBase64String(this int i)
        {
            return System.Convert.ToBase64String(System.BitConverter.GetBytes(i));
        }
        
        public static string StringToBase64String(this string str)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        }
        
        public static string TakeFirstLetters(this string str, int count)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= count)
                return str;
            var returna = str.Substring(0, count);
            return returna;
        }
    }
}