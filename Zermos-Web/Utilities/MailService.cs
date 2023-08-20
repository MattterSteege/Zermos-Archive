using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Zermos_Web.Utilities
{
    public static class MailgunService
    {
        static SmtpClient _smtp;
    
        public static async Task Initialize()
        {
            _smtp = new SmtpClient();
            await _smtp.ConnectAsync("smtp.eu.mailgun.org", 587, SecureSocketOptions.StartTls);
            await _smtp.AuthenticateAsync("no-reply@mail.kronk.tech", "1a41c6963490e6ef58f3444a5e9b2f29-c30053db-ef71dc20");
        }
        
        public static async Task Deinitialize()
        {
            await _smtp.DisconnectAsync(true);
        }
        
        /// <param name="login_mail">this bool means: true, send login mail, false: send creation mail</param>
        public static async Task SendEmail(string to, string subject, string url, bool login_mail)
        {
            _smtp = new SmtpClient();
            await _smtp.ConnectAsync("smtp.eu.mailgun.org", 587, SecureSocketOptions.StartTls);
            await _smtp.AuthenticateAsync("no-reply@mail.kronk.tech", "1a41c6963490e6ef58f3444a5e9b2f29-c30053db-ef71dc20");

            string message;
            if (login_mail)
            {
                message = "<!DOCTYPE html><html lang=\"en\"><head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" charset=\"UTF-8\"/> <title>Zermos</title> <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700\" rel=\"stylesheet\"/> <script src=\"https://kit.fontawesome.com/052f5fa8f8.js\" crossorigin=\"anonymous\"></script> <style>body{background: #f8f9fa;}.middle{width: 100%; background-color: #ffffff; display: flex; flex-direction: column;}.zermos{font-family: 'Open Sans', sans-serif; font-weight: bold; font-size: 3rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.content{font-family: 'Open Sans', sans-serif; font-weight: 300; font-size: 1rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 400; font-size: 1rem; color: #344767; width: 100%; text-align: center; margin: 0;}.verify-button{font-family: 'Open Sans', sans-serif; font-weight: 600; font-size: 1.5rem; color: #ffffff; width: 50%; height: auto; background-color: #344767; border-radius: 14px; padding: 1rem; margin: 1rem 25%; border: none; cursor: pointer; text-decoration: none; text-align: center;}.verify-button:hover{background-color: #2c3e50;}.verify-button:active{background-color: #344767;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 200; font-size: 0.8rem; color: #344767; width: 100%; text-align: center; margin: 0; padding-bottom: 1rem;}</style></head><body><div class=\"middle\"><h1 class=\"zermos\">Zermos</h1> <p class=\"content\">Je hebt zojuist een account aangemaakt bij <strong>Zermos</strong>. Daarom is er een e-mail verzonden om te verifiëren dat je daadwerkelijk een account wilt aanmaken.</p><p class=\"content\">Bevestig je e-mailadres door op de onderstaande knop te klikken. Deze stap voegt extra beveiliging toe aan je account door te verifiëren dat jij de eigenaar bent van dit e-mailadres.</p><a href='";
                message += url;
                message += "' class=\"verify-button\">Log nu in!</a> <p class=\"valid-time\">P.S. deze link is 10 minuten geldig.</p></div></body></html>";
            }
            else
            {
                message = "<!DOCTYPE html><html lang=\"en\"><head> <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" charset=\"UTF-8\"/> <title>Zermos</title> <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700\" rel=\"stylesheet\"/> <script src=\"https://kit.fontawesome.com/052f5fa8f8.js\" crossorigin=\"anonymous\"></script> <style>body{background: #f8f9fa;}.middle{width: 100%; background-color: #ffffff; display: flex; flex-direction: column;}.zermos{font-family: 'Open Sans', sans-serif; font-weight: bold; font-size: 3rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.content{font-family: 'Open Sans', sans-serif; font-weight: 300; font-size: 1rem; color: #344767; width: calc(100% - 2rem); text-align: center; margin: 0; padding: 1rem;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 400; font-size: 1rem; color: #344767; width: 100%; text-align: center; margin: 0;}.verify-button{font-family: 'Open Sans', sans-serif; font-weight: 600; font-size: 1.5rem; color: #ffffff; width: 50%; height: auto; background-color: #344767; border-radius: 14px; padding: 1rem; margin: 1rem 25%; border: none; cursor: pointer; text-decoration: none; text-align: center;}.verify-button:hover{background-color: #2c3e50;}.verify-button:active{background-color: #344767;}.valid-time{font-family: 'Open Sans', sans-serif; font-weight: 200; font-size: 0.8rem; color: #344767; width: 100%; text-align: center; margin: 0; padding-bottom: 1rem;}</style></head><body><div class=\"middle\"><h1 class=\"zermos\">Zermos</h1> <p class=\"content\">Je hebt zojuist een account aangemaakt bij <strong>Zermos</strong>. Daarom is er een e-mail verzonden om te verifiëren dat je daadwerkelijk een account wilt aanmaken.</p><p class=\"content\">Bevestig je e-mailadres door op de onderstaande knop te klikken. Deze stap voegt extra beveiliging toe aan je account door te verifiëren dat jij de eigenaar bent van dit e-mailadres.</p><a href='";
                message += url;
                message += "' class=\"verify-button\">Log nu in!</a> <p class=\"valid-time\">P.S. deze link is 10 minuten geldig.</p></div></body></html>";
            }
            
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse("Zermos <no-reply@mail.kronk.tech>"));
            mimeMessage.To.Add(MailboxAddress.Parse(to));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };
    
            await _smtp.SendAsync(mimeMessage);
            
            await _smtp.DisconnectAsync(true);
        }
    }
}