using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace zermos_authentication_service.Controllers
{
    [ApiController]
    [Route("somtoday")]
    public class SOMtoday : ControllerBase
    {
        
        public string Index()
        {
            return "These API endpoints are for the SOMtoday authentication service.\nAuthenticate\nGrades";
        }
        
        #region authenticate
        
        [Route("authenticate")]
        public string authenticateSomtodayUser(string username, string password)
        {
            if (username == null || password == null) return "set username and password";
            
            
            string[] codes = GenerateTokens();
            
            string CodeVerifier = codes[0];
            string CodeChallenge = codes[1];
            
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(handler, false);

            using var httpClient = client;
            string baseUrl = string.Format(
                "https://inloggen.somtoday.nl/oauth2/authorize?redirect_uri=somtodayleerling://oauth/callback&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&response_type=code&state={0}&scope=openid&tenant_uuid={1}&session=no_session&code_challenge={2}&code_challenge_method=S256",
                generateRandomString(8), "c23fbb99-be4b-4c11-bbf5-57e7fc4f4388", CodeChallenge);
            var response = client.GetAsync(baseUrl).Result;

            if (response.StatusCode != HttpStatusCode.Found) return "failed at first request";

            HttpHeaders headers = response.Headers;
            IEnumerable<string> values;
            if (!headers.TryGetValues("Location", out values)) return "failed at first request - no location header";


            Uri myUri = new Uri(values.First());

            string authToken = ParseQuery(myUri.Query).Get("auth");

            FormUrlEncodedContent Content;
            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"loginLink", "x"},
                {"usernameFieldPanel:usernameFieldPanel_body:usernameField", username}
            });

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("origin", "https://inloggen.somtoday.nl");
            HttpResponseMessage response2 = client
                .PostAsync($"https://inloggen.somtoday.nl/?-1.-panel-signInForm&auth={authToken}", Content).Result;

            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"passwordFieldPanel:passwordFieldPanel_body:passwordField", password},
                {"loginLink", "x"}
            });

            HttpResponseMessage response3 = client
                .PostAsync($"https://inloggen.somtoday.nl/login?1-1.-passwordForm&auth={authToken}", Content)
                .Result;

            headers = response3.Headers;
            IEnumerable<string> values3;
            if (!headers.TryGetValues("Location", out values3)) return "failed at second request - no location header";

            myUri = new Uri(values3.First());

            string authCode = ParseQuery(myUri.Query).Get("code");

            string ResponseHTML = response3.Content.ReadAsStringAsync().Result;

            Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"", ""}
            });

            client.DefaultRequestHeaders.Remove("origin");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            HttpResponseMessage response4 = client.PostAsync(
                $"https://inloggen.somtoday.nl/oauth2/token?grant_type=authorization_code&session=no_session&scope=openid&client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2&tenant_uuid=c23fbb99-be4b-4c11-bbf5-57e7fc4f4388&code={authCode}&code_verifier={CodeVerifier}",
                Content).Result;

            return response4.Content.ReadAsStringAsync().Result;
        }

        string generateRandomString(int count)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[count];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[new Random().Next(0, chars.Length)];
            }

            return new String(stringChars);
        }
        
        string[] GenerateTokens()
        {
            string[] codes = new string[2];
             
            codes[0] = GenerateNonce();
            codes[1] = GenerateCodeChallenge(codes[0]);
            
            return codes;
        }

        private static string GenerateNonce()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var nonce = new char[128];
            for (int i = 0; i < nonce.Length; i++)
            {
                nonce[i] = chars[new Random().Next(0, chars.Length)];
            }

            return new string(nonce);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }
        
        static NameValueCollection ParseQuery(string query)
        {
            NameValueCollection queryParameters = new NameValueCollection();
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?"))
                {
                    query = query.Remove(0, 1);
                }

                foreach (string parameter in query.Split('&'))
                {
                    string[] parts = parameter.Split('=');
                    string key = parts[0];
                    string value = parts.Length > 1 ? parts[1] : "";
                    queryParameters.Add(key, value);
                }
            }
            return queryParameters;
        }
        #endregion

        #region grades
        [Route("grades")]
        public string getGrades(string token, string studentId, string begintNaOfOp)
        {
            if (token == null) return "set token";

            string baseUrl = $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{studentId}?begintNaOfOp={begintNaOfOp}";
            baseUrl += "&additional=berekendRapportCijfer";
            baseUrl += "&additional=samengesteldeToetskolomId";
            baseUrl += "&additional=resultaatkolomId";
            baseUrl += "&additional=cijferkolomId";
            baseUrl += "&additional=toetssoortnaam";
            baseUrl += "&additional=huidigeAnderVakKolommen";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Range", "items=0-99");
            
            HttpResponseMessage response = client.GetAsync(baseUrl).Result;
            
            //newtsoft json
            SomtodayGrades grades = JsonConvert.DeserializeObject<SomtodayGrades>(response.Content.ReadAsStringAsync().Result);

            client.DefaultRequestHeaders.Remove("Range");
            client.DefaultRequestHeaders.Add("Range", $"items=100-199");
            
            response = client.GetAsync(baseUrl).Result;
            
            grades.items.AddRange(JsonConvert.DeserializeObject<SomtodayGrades>(response.Content.ReadAsStringAsync().Result).items);

            return JsonConvert.SerializeObject(Sort(grades));
            //return grades.items.Count.ToString();
        }
        
        [Route("gradesById")]
        public string getGradesById(string token, Int64 vakId ,string studentId, string begintNaOfOp)
        {
            if (token == null) return "set token";

            string baseUrl = $"https://api.somtoday.nl/rest/v1/resultaten/huidigVoorLeerling/{studentId}?begintNaOfOp={begintNaOfOp}";
            baseUrl += "&additional=berekendRapportCijfer";
            baseUrl += "&additional=samengesteldeToetskolomId";
            baseUrl += "&additional=resultaatkolomId";
            baseUrl += "&additional=cijferkolomId";
            baseUrl += "&additional=toetssoortnaam";
            baseUrl += "&additional=huidigeAnderVakKolommen";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Range", "items=0-99");
            
            HttpResponseMessage response = client.GetAsync(baseUrl).Result;
            
            //newtsoft json
            SomtodayGrades grades = JsonConvert.DeserializeObject<SomtodayGrades>(response.Content.ReadAsStringAsync().Result);

            client.DefaultRequestHeaders.Remove("Range");
            client.DefaultRequestHeaders.Add("Range", $"items=100-199");
            
            response = client.GetAsync(baseUrl).Result;
            
            grades.items.AddRange(JsonConvert.DeserializeObject<SomtodayGrades>(response.Content.ReadAsStringAsync().Result).items);

            //Sort(grades);
            //then filter vakId
            grades.items.RemoveAll(x => (int) x.vak.links[0].id != vakId);
            Sort(grades);
            return JsonConvert.SerializeObject(grades);

            //return grades.items.Count.ToString();
        }
        
        SomtodayGrades Sort(SomtodayGrades grades)
        {
            grades.items.RemoveAll(x => x.geldendResultaat == null);
            grades.items.RemoveAll(x => string.IsNullOrEmpty(x.omschrijving) && x.weging == 0);
            grades.items = grades.items.OrderBy(x => x.datumInvoer).ToList();
            return grades;
        }

        public class SomtodayGrades
        {
            public List<Item> items { get; set; }
        }

        public class Item
        {
            //[JsonProperty("$type")] public string Type { get; set; }
            public List<Link> links { get; set; }
            public List<Permission> permissions { get; set; }
            public string herkansingstype { get; set; }
            public DateTime datumInvoer { get; set; }
            public bool teltNietmee { get; set; }
            public bool toetsNietGemaakt { get; set; }
            public int leerjaar { get; set; }
            public int periode { get; set; }
            public int weging { get; set; }
            public int examenWeging { get; set; }
            public bool isExamendossierResultaat { get; set; }
            public bool isVoortgangsdossierResultaat { get; set; }
            public string type { get; set; }
            public string omschrijving { get; set; }
            public Vak vak { get; set; }
            public int volgnummer { get; set; }
            public bool vrijstelling { get; set; }
            public string resultaat { get; set; }
            public string geldendResultaat { get; set; }
        }

        public class Link
        {
            public Int64 id { get; set; }
            public string rel { get; set; }
            public string type { get; set; }
            public string href { get; set; }
        }

        public class Permission
        {
            public string full { get; set; }
            public string type { get; set; }
            public List<string> operations { get; set; }
            public List<string> instances { get; set; }
        }


        public class Vak
        {
            public List<Link> links { get; set; }
            public List<Permission> permissions { get; set; }
            public string afkorting { get; set; }
            public string naam { get; set; }
        }
        #endregion

        #region homework
        [Route("huiswerk")]
        public string getHomework(string token, string begintNaOfOp)
        {
            if (token == null) return "set token";

            string baseUrl = $"https://api.somtoday.nl/rest/v1/studiewijzeritemafspraaktoekenningen?begintNaOfOp={begintNaOfOp}&additional=swigemaaktVinkjes&additional=huiswerkgemaakt&additional=leerlingen";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("Range", "items=0-99");
            
            HttpResponseMessage response = client.GetAsync(baseUrl).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        #endregion
        
        #region student
        [Route("student")]
        public string getStudent(string token)
        {
            if (token == null) return "set token";

            string baseUrl = $"https://api.somtoday.nl/rest/v1/leerlingen/";
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            
            HttpResponseMessage response = client.GetAsync(baseUrl).Result;
            
            return response.Content.ReadAsStringAsync().Result;
        }
        #endregion

        #region refresh
        [Route("refresh")]
        public string refresh(string refreshToken)
        {
            //POST: "https://inloggen.somtoday.nl/oauth2/token,
            //HEADERS:
            //grant_type=refresh_token
            //refresh_token=REFRESH_TOKEN
            //client_id=D50E0C06-32D1-4B41-A137-A9A850C892C2
            //scope=openid
            
            string baseUrl = $"https://inloggen.somtoday.nl/oauth2/token";
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", "D50E0C06-32D1-4B41-A137-A9A850C892C2"),
                new KeyValuePair<string, string>("scope", "openid")
            });
            
            HttpResponseMessage response = client.PostAsync(baseUrl, content).Result;
            
            return response.Content.ReadAsStringAsync().Result;
        }
        #endregion
        
    }
}