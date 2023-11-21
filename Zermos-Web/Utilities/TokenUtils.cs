using System;
using System.Linq;
using System.Text;

namespace Zermos_Web.Utilities
{
    public static class TokenUtils
    {
        private static readonly Random Random = new Random();

        /// <param name="token">the token that you want to check</param>
        /// <returns>
        ///     true: if token is valid or doesn't expire<br></br>
        ///     false: if the token is expired or invalid
        /// </returns>
        public static bool CheckToken(string token)
        {
            if (token == null) return false;

            // Split the token into header, payload, and signature parts
            var parts = token.Split('.');
            if (parts.Length != 3)
                // Invalid token format
                return false;

            // Base64Url decode the payload
            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

            // Parse the exp value as a long
            var expIndex = payloadJson.IndexOf("\"exp\":", StringComparison.Ordinal);
            if (expIndex == -1)
                // exp property not found
                return false;
            var startIndex = expIndex + 6; // 7 is the length of "\"exp\":"
            var endIndex = payloadJson.IndexOf(',', startIndex);
            if (endIndex == -1) endIndex = payloadJson.Length - 1;
            var expValue = payloadJson.Substring(startIndex, endIndex - startIndex);
            if (!int.TryParse(expValue, out var exp))
                // Failed to parse exp value
                return false;

            // Check if the current time is before the expiry time of the token
            var expDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(exp).ToLocalTime();
            return DateTime.Now < expDate;
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2:
                    output += "==";
                    break; // Two pad chars
                case 3:
                    output += "=";
                    break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }

            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
        
        public static DateTime GetTokenExpiration(string token)
        {
            if (token == null) return DateTime.MinValue;

            // Split the token into header, payload, and signature parts
            var parts = token.Split('.');
            if (parts.Length != 3)
                // Invalid token format
                return DateTime.MinValue;

            // Base64Url decode the payload
            var payload = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

            // Parse the exp value as a long
            var expIndex = payloadJson.IndexOf("\"exp\":", StringComparison.Ordinal);
            if (expIndex == -1)
                // exp property not found
                return DateTime.MinValue;
            var startIndex = expIndex + 6; // 7 is the length of "\"exp\":"
            var endIndex = payloadJson.IndexOf(',', startIndex);
            if (endIndex == -1) endIndex = payloadJson.Length - 1;
            var expValue = payloadJson.Substring(startIndex, endIndex - startIndex);
            if (!int.TryParse(expValue, out var exp))
                // Failed to parse exp value
                return DateTime.MinValue;

            // Check if the current time is before the expiry time of the token
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(exp).ToLocalTime();
        }

        [Flags]
        public enum RandomStringType
        {
            LowerCase,
            UpperCase,
            Numbers,
            SpecialCharacters
        }


        /* This is the probablility table when RandomStringType is set to LowerCase, UpperCase and Numbers
        
        | Length | Probability              | Probability (%)              |
        |--------|--------------------------|------------------------------|
        | 1      | 1.61 x 10^-2             | 1.61 x 10^-2 %              |
        | 2      | 2.60 x 10^-4             | 2.60 x 10^-4 %              |
        | 3      | 4.19 x 10^-6             | 4.19 x 10^-6 %              |
        | 4      | 6.75 x 10^-8             | 6.75 x 10^-8 %              |
        | 5      | 1.09 x 10^-9             | 1.09 x 10^-9 %              |
        | 6      | 1.76 x 10^-11            | 1.76 x 10^-11 %             |
        | 7      | 2.84 x 10^-13            | 2.84 x 10^-13 %             |
        | 8      | 4.58 x 10^-15            | 4.58 x 10^-15 %             |
        | 9      | 7.39 x 10^-17            | 7.39 x 10^-17 %             |
        | 10     | 1.19 x 10^-18            | 1.19 x 10^-18 %             |
        | 11     | 1.92 x 10^-20            | 1.92 x 10^-20 %             |
        | 12     | 3.10 x 10^-22            | 3.10 x 10^-22 %             |
        | 13     | 5.00 x 10^-24            | 5.00 x 10^-24 %             |
        | 14     | 8.06 x 10^-26            | 8.06 x 10^-26 %             |
        | 15     | 1.30 x 10^-27            | 1.30 x 10^-27 %             |
        | 16     | 2.10 x 10^-29            | 2.10 x 10^-29 %             |
        | 17     | 3.39 x 10^-31            | 3.39 x 10^-31 %             |
        | 18     | 5.47 x 10^-33            | 5.47 x 10^-33 %             |
        | 19     | 8.82 x 10^-35            | 8.82 x 10^-35 %             |
        | 20     | 1.42 x 10^-36            | 1.42 x 10^-36 %             | 
        
        The calculation for this table is: (1/26)^x * 100 where x is the length of the string */

        /// <summary>
        /// Show implementation of this method to show the probability of a string being generated.
        /// </summary>
        public static string RandomString(int length = 6, RandomStringType type = RandomStringType.UpperCase)
        {
            var chars = "";
            switch (type)
            {
                case RandomStringType.LowerCase:
                    chars += "abcdefghijklmnopqrstuvwxyz";
                    break;
                case RandomStringType.UpperCase:
                    chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case RandomStringType.Numbers:
                    chars += "0123456789";
                    break;
                case RandomStringType.SpecialCharacters:
                    chars += "!\"§$%&/()=?`´*+~#'-_.:,;<>|\\";
                    break;
            }
            
            var result = "";
            for (var i = 0; i < length; i++)
                result += chars[Random.Next(0, chars.Length)];


            return result;
        }
        
        public static bool IsValidBase64String(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Check if the input length is a multiple of 4
            if (input.Length % 4 != 0)
                return false;

            // Check if the input contains only valid Base64 characters
            if (!input.All(c => Char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='))
                return false;

            try
            {
                // Attempt to decode the input to verify it's a valid Base64 string
                Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}