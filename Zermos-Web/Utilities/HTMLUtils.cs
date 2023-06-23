using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Zermos_Web.Utilities
{
    public static class HTMLUtils
    {
        /// <summary>
        ///     Replaces all HTML entities (h1, p, etc.) in a string with their corresponding characters.
        /// </summary>
        /// <param name="input">The string to replace the HTML entities in.</param>
        /// <returns>The string with the HTML entities replaced.</returns>
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        ///     Replaces all HTML entities (&quot;, &amp;, etc.) in a string with their corresponding characters (", &, etc.).
        /// </summary>
        /// <param name="htmlText">The string to replace the HTML entities in.</param>
        /// <returns>The string with the HTML entities replaced.</returns>
        public static string ReplaceHtmlEntities(string htmlText)
        {
            var entityMap = new Dictionary<string, char>
            {
                {"&quot;", '"'},
                {"&amp;", '&'},
                {"&lt;", '<'},
                {"&gt;", '>'},
                {"&nbsp;", ' '},
                {"&iexcl;", '¡'},
                {"&cent;", '¢'},
                {"&pound;", '£'},
                {"&curren;", '¤'},
                {"&yen;", '¥'},
                {"&brvbar;", '¦'},
                {"&sect;", '§'},
                {"&uml;", '¨'},
                {"&copy;", '©'},
                {"&ordf;", 'ª'},
                {"&laquo;", '«'},
                {"&not;", '¬'},
                {"&reg;", '®'},
                {"&macr;", '¯'},
                {"&deg;", '°'},
                {"&plusmn;", '±'},
                {"&sup2;", '²'},
                {"&sup3;", '³'},
                {"&acute;", '´'},
                {"&micro;", 'µ'},
                {"&para;", '¶'},
                {"&middot;", '·'},
                {"&cedil;", '¸'},
                {"&sup1;", '¹'},
                {"&ordm;", 'º'},
                {"&raquo;", '»'},
                {"&frac14;", '¼'},
                {"&frac12;", '½'},
                {"&frac34;", '¾'},
                {"&iquest;", '¿'},
                {"&Agrave;", 'À'},
                {"&Aacute;", 'Á'},
                {"&Acirc;", 'Â'},
                {"&Atilde;", 'Ã'},
                {"&Auml;", 'Ä'},
                {"&Aring;", 'Å'},
                {"&AElig;", 'Æ'},
                {"&Ccedil;", 'Ç'},
                {"&Egrave;", 'È'},
                {"&Eacute;", 'É'},
                {"&Ecirc;", 'Ê'},
                {"&Euml;", 'Ë'},
                {"&Igrave;", 'Ì'},
                {"&Iacute;", 'Í'},
                {"&Icirc;", 'Î'},
                {"&Iuml;", 'Ï'},
                {"&ETH;", 'Ð'},
                {"&Ntilde;", 'Ñ'},
                {"&Ograve;", 'Ò'},
                {"&Oacute;", 'Ó'},
                {"&Ocirc;", 'Ô'},
                {"&Otilde;", 'Õ'},
                {"&Ouml;", 'Ö'},
                {"&times;", '×'},
                {"&Oslash;", 'Ø'},
                {"&Ugrave;", 'Ù'},
                {"&Uacute;", 'Ú'},
                {"&Ucirc;", 'Û'},
                {"&Uuml;", 'Ü'},
                {"&Yacute;", 'Ý'},
                {"&THORN;", 'Þ'},
                {"&szlig;", 'ß'},
                {"&agrave;", 'à'},
                {"&aacute;", 'á'},
                {"&acirc;", 'â'},
                {"&atilde;", 'ã'},
                {"&auml;", 'ä'},
                {"&aring;", 'å'},
                {"&aelig;", 'æ'},
                {"&ccedil;", 'ç'},
                {"&egrave;", 'è'},
                {"&eacute;", 'é'},
                {"&ecirc;", 'ê'},
                {"&euml;", 'ë'},
                {"&igrave;", 'ì'},
                {"&iacute;", 'í'},
                {"&icirc;", 'î'},
                {"&iuml;", 'ï'},
                {"&eth;", 'ð'},
                {"&ntilde;", 'ñ'},
                {"&ograve;", 'ò'},
                {"&oacute;", 'ó'},
                {"&ocirc;", 'ô'},
                {"&otilde;", 'õ'},
                {"&ouml;", 'ö'},
                {"&divide;", '÷'},
                {"&oslash;", 'ø'},
                {"&ugrave;", 'ù'},
                {"&uacute;", 'ú'},
                {"&ucirc;", 'û'},
                {"&uuml;", 'ü'},
                {"&yacute;", 'ý'},
                {"&thorn;", 'þ'},
                {"&yuml;", 'ÿ'}
            };

            var regex = new Regex("(&[A-Za-z]+;)");

            var matches = regex.Matches(htmlText ?? "");

            foreach (Match match in matches)
                if (entityMap.TryGetValue(match.Value, out var replacementChar))
                    htmlText = htmlText.Replace(match.Value, replacementChar.ToString());

            return htmlText ?? "";
        }

        /// <summary>
        ///     Parses a query string into a NameValueCollection.
        /// </summary>
        /// <param name="query">the url query string</param>
        /// <returns>the NameValueCollection containing the query parameters</returns>
        public static NameValueCollection ParseQuery(string query)
        {
            var queryParameters = new NameValueCollection();
            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?")) query = query.Remove(0, 1);

                foreach (var parameter in query.Split('&'))
                {
                    var parts = parameter.Split('=');
                    var key = parts[0];
                    var value = parts.Length > 1 ? parts[1] : "";
                    queryParameters.Add(key, value);
                }
            }

            return queryParameters;
        }
        
        /// <summary>
        /// Converts links found in the provided text to a modified format.
        /// </summary>
        /// <param name="text">The input text.</param>
        /// <returns>The modified text with links in the modified format.</returns>
        public static string ConvertLinksInText(string text)
        {
            // Define the regex pattern to match URLs
            string pattern = @"(https?://\S+)";

            // Replace URLs with modified anchor tags
            string convertedText = Regex.Replace(text, pattern, match =>
            {
                string originalLink = match.Value;
                Uri uri = new Uri(originalLink);
                string domain = uri.Host;
                string modifiedLink = $"<a href='{originalLink}'>{domain}</a>";
                return modifiedLink;
            });

            return convertedText;
        }
    }
}