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
    }
}