using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Zermos_Web.Utilities
{
    public class MailgunService
    {
        private readonly string APIKEY;
        private readonly HttpClient client;
        private readonly string DOMAIN;

        public MailgunService(string apiKey, string domainName)
        {
            APIKEY = apiKey;
            DOMAIN = domainName;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("api" + ":" + APIKEY)));
        }

        public async Task<HttpResponseMessage> SendSimpleMessage(string from, string to, string subject, string text,
            string html)
        {
            var form = new Dictionary<string, string>();
            form["from"] = from;
            form["to"] = to;
            form["subject"] = subject;
            form["text"] = text;
            form["html"] = html;

            var response = await client.PostAsync("https://api.mailgun.net/v3/" + DOMAIN + "/messages",
                new FormUrlEncodedContent(form));

            if (response.StatusCode == HttpStatusCode.OK)
                return response;

            Debug.WriteLine("StatusCode: " + response.StatusCode);
            Debug.WriteLine("ReasonPhrase: " + response.ReasonPhrase);
            return null;
        }
    }
}