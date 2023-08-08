// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

#nullable disable

namespace Infrastructure.Entities
{
    public class user
    {
        [Key] public string email { get; set; }
        public string name { get; set; }
        public string school_id { get; set; }
        public string zermelo_access_token { get; set; }
        public DateTime? zermelo_access_token_expires_at { get; set; }
        public string somtoday_access_token { get; set; }
        public string somtoday_refresh_token { get; set; }
        public string somtoday_student_id { get; set; }
        public string infowijs_access_token { get; set; }
        public string theme { get; set; }
        public string custom_huiswerk { get; set; }
        
        //cache related
        public string cached_somtoday_grades { get; set; }
        public string cached_somtoday_homework { get; set; }
        public string cached_infowijs_calendar { get; set; }
        public string cached_infowijs_news { get; set; }
        public string cached_school_informationscreen { get; set; }
        public string cached_zermelo_schedule { get; set; }

        //auth related
        public string VerificationToken { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsVerified => VerifiedAt.HasValue;

        //API related
        // public string? ApiKey { get; set; }
        // public DateTime? ApiKeyExpiresAt { get; set; }
        // public bool ApiKeyIsExpired => ApiKeyExpiresAt.HasValue && ApiKeyExpiresAt <= DateTime.UtcNow;

        //koppelingen related
        public bool is_zermelo_linked => zermelo_access_token != null;
        public bool is_somtoday_linked => tokenUtils.CheckToken(somtoday_refresh_token);
        public bool is_infowijs_linked => tokenUtils.CheckToken(infowijs_access_token);
    }

    static class tokenUtils
    {
        //mark as not for EF
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
    }
}