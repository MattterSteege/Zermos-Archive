using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CheckTokenExpirationDate
{
    /// <param name="token">the token that you want to check</param>
    /// <returns>true: if token is valid or doesn't expire<br></br>
    /// false: if the token is expired</returns>
    public bool CheckToken(string token)
    {
        if (token == null) return true;
        
        string[] tokenParts = token.Split('.');
        string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(tokenParts[1]));
        JObject payload = JsonConvert.DeserializeObject<JObject>(payloadJson);

        if (payload.ContainsKey("exp"))
        {
            int exp = (int)payload["exp"];
            if (DateTime.UtcNow > exp.ToDateTime())
                return false;
            return true;

        }
        return true;
    }

    static byte[] Base64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(s);
    }
}
