using System;
using System.Text;

public class CheckTokenExpirationDate
{
    /// <param name="token">the token that you want to check</param>
    /// <returns>true: if token is valid or doesn't expire<br></br>
    /// false: if the token is expired</returns>
    public bool CheckToken(string token)
    {
        // Split the token into header, payload, and signature parts
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            // Invalid token format
            return false;
        }

        // Base64Url decode the payload
        var payload = parts[1];
        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

        // Parse the exp value as a long
        var expIndex = payloadJson.IndexOf("\"exp\":", StringComparison.Ordinal);
        if (expIndex == -1)
        {
            // exp property not found
            return false;
        }
        var startIndex = expIndex + 6; // 7 is the length of "\"exp\":"
        var endIndex = payloadJson.IndexOf(',', startIndex);
        if (endIndex == -1)
        {
            endIndex = payloadJson.Length - 1;
        }
        var expValue = payloadJson.Substring(startIndex, endIndex - startIndex);
        if (!int.TryParse(expValue, out var exp))
        {
            // Failed to parse exp value
            return false;
        }

        // Check if the current time is before the expiry time of the token
        return DateTime.Now < exp.ToDateTime();
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var output = input;
        output = output.Replace('-', '+'); // 62nd char of encoding
        output = output.Replace('_', '/'); // 63rd char of encoding
        switch (output.Length % 4) // Pad with trailing '='s
        {
            case 0: break; // No pad chars in this case
            case 2: output += "=="; break; // Two pad chars
            case 3: output += "="; break; // One pad char
            default: throw new Exception("Illegal base64url string!");
        }
        var converted = Convert.FromBase64String(output); // Standard base64 decoder
        return converted;
    }
}