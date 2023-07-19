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

        /// <summary>
        /// This method balances HTML tags in the provided input string, so it adds any missing closing tags <hello>world -> <hello>world</hello>
        /// </summary>
        /// <param name="input">The input string to balance.</param>
        /// <returns>The balanced string.</returns>
        public static string BalanceHtmlTags(string input)
        {
            // Regular expression to match any HTML tag
            Regex tagRegex = new Regex(@"<([a-zA-Z]+)(?:\s*[^>]*)>?");

            // Find all the matches of HTML tags in the input
            MatchCollection tagMatches = tagRegex.Matches(input);

            // Create a stack to keep track of opening tags
            Stack<string> tagStack = new Stack<string>();

            foreach (Match match in tagMatches)
            {
                string tag = match.Value;
                string tagName = match.Groups[1].Value;

                if (!tag.StartsWith("</"))
                {
                    // If it's an opening tag, push it onto the stack
                    tagStack.Push(tag);
                }
                else
                {
                    // If it's a closing tag, check if there is a corresponding opening tag
                    if (tagStack.Count > 0 && tagStack.Peek().StartsWith($"<{tagName}"))
                    {
                        // Pop the corresponding opening tag from the stack
                        tagStack.Pop();
                    }
                    else
                    {
                        // If no corresponding opening tag is found, add the opening tag before the closing tag
                        input = input.Insert(match.Index, $"<{tagName}>");
                    }
                }
            }

            // Add any missing closing tags at the end of the input
            while (tagStack.Count > 0)
            {
                string remainingTag = tagStack.Pop();
                
                //does the remaining tag have a closing tag?
                if (!remainingTag.EndsWith(">"))
                {
                    input += ">";
                }

                string tagName = remainingTag.TrimStart('<').TrimEnd('>');
                input += $"</{tagName}>";
            }

            return input;
        }
    }
}