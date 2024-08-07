using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

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
        
        public static string Base64StringToString(this string base64)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64));
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

        public static string ReplaceNewLines(this string str)
        {
            // Replace newlines with spaces
            string result = str.Replace("\n", " ").Replace("\r", "");

            // Remove newline if it has a space before it
            result = Regex.Replace(result, @"(\s+)\n", "$1");

            return result;
        }
        
        public static string DecodeBase64Url(string encodedString)
        {
            encodedString = encodedString.Replace('-', '+').Replace('_', '/');

            int paddingNeeded = encodedString.Length % 4;
            if (paddingNeeded > 0)
            {
                encodedString += new string('=', 4 - paddingNeeded);
            }

            byte[] data = Convert.FromBase64String(encodedString);

            string decodedString = Encoding.UTF8.GetString(data);

            //now url decode
            decodedString = HttpUtility.UrlDecode(decodedString);
            
            return decodedString;
        }
        
        /// <summary>
        /// Encodes a string to a base62 string.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>The base62 encoded string.</returns>
        public static string shortenUUID(this string uuid)
        {
            // Convert the UUID to a BigInteger
            BigInteger bigInt = new BigInteger(uuid.Replace("-", "").Select(c => (byte)c).ToArray());
            
            // Define the base62 characters
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            
            // Define the result string
            string result = "";
            
            // While the bigInt is greater than 0
            while (bigInt > 0)
            {
                // Get the remainder of the bigInt divided by 62
                int remainder = (int)(bigInt % 62);
                
                // Add the character at the remainder index to the result string
                result = chars[remainder] + result;
                
                // Divide the bigInt by 62
                bigInt /= 62;
            }
            
            // Return the result string
            return result;
        }

        /// <summary>
        /// Decodes a base62 string to a string.
        /// </summary>
        /// <param name="str">The base62 string to decode.</param>
        /// <returns>The decoded string.</returns>
        public static string expandUUID(this string base62)
        {
            // Define the base62 characters
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            // Define the result BigInteger
            BigInteger result = 0;

            // For each character in the base62 string
            foreach (char c in base62)
            {
                // Multiply the result by 62
                result *= 62;

                // Add the index of the character in the base62 characters to the result
                result += chars.IndexOf(c);
            }

            // Convert the result to a byte array
            byte[] bytes = result.ToByteArray();

            // Define the result string
            string resultStr = "";

            // For each byte in the byte array
            foreach (byte b in bytes)
            {
                // Add the byte to the result string
                resultStr += (char) b;
            }

            //add back the dashes
            resultStr = resultStr.Insert(8, "-");
            resultStr = resultStr.Insert(13, "-");
            resultStr = resultStr.Insert(18, "-");
            resultStr = resultStr.Insert(23, "-");
            
            // Return the result string
            return resultStr;
        }
    }
}