using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Zermos_Web.Utilities
{
    // public static class MailgunService
    // {
    //     static SmtpClient _smtp;
    //
    //     public static async Task Initialize()
    //     {
    //         _smtp = new SmtpClient();
    //         await _smtp.ConnectAsync("smtp.eu.mailgun.org", 587, SecureSocketOptions.StartTls);
    //         await _smtp.AuthenticateAsync("no-reply@mail.kronk.tech", "1a41c6963490e6ef58f3444a5e9b2f29-c30053db-ef71dc20");
    //     }
    //     
    //     public static async Task Deinitialize()
    //     {
    //         await _smtp.DisconnectAsync(true);
    //     }
    //
    //     public static async Task sendEmail(string email, string subject, string url)
    //     {
    //         if (_smtp == null)
    //         {
    //             await Initialize();
    //         }
    //         
    //         MimeMessage mimeMessage = new MimeMessage();
    //         mimeMessage.From.Add(MailboxAddress.Parse("no-reply@mail.kronk.tech"));
    //         mimeMessage.To.Add(MailboxAddress.Parse(email));
    //         mimeMessage.Subject = subject;
    //         mimeMessage.Body = new TextPart(TextFormat.Html)
    //         {
    //             Text = message
    //         };
    //
    //         await _smtp.SendAsync(mimeMessage);
    //     }
    // }
    
    public static class MailgunService
    {
        private static HttpClient _httpClient;
        private static readonly string _apiKey = "f598296997c02a39e837b2cb78db7227-4e034d9e-e0228685";
        private static readonly string _domain = "mail.kronk.tech";

        public static async Task Initialize()
        {

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.eu.mailgun.net/v3/mail.kronk.tech/messages"),
                DefaultRequestHeaders =
                {
                    Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YXBpOmFhMGY5YTJlOGE0ZGQ1MjRhZGJlZTQwMWMzYmJkMDgyLTRlMDM0ZDllLTdiNGM3NDNk")
                }
            };
        }

        public static async Task<bool> SendEmail(string to, string subject, string template, string url)
        {
            if (_httpClient == null)
            {
                await Initialize();
            }

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("Zermos <no-reply@mail.kronk.tech>"), "from");
            formData.Add(new StringContent(to), "to");
            formData.Add(new StringContent(subject), "subject");
            formData.Add(new StringContent(template), "template");
            formData.Add(new StringContent(url), "h:X-Mailgun-Variables");

            var response = await _httpClient.PostAsync("", formData);
            return response.IsSuccessStatusCode;
        }
    }
}