using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class AntoniusNews : BetterHttpClient
{
    [ContextMenu("test")]
    public List<AntoniusNewsItem> GetNews()
    {
        List<AntoniusNewsItem> items = new List<AntoniusNewsItem>();

        return  new CoroutineWithData<List<AntoniusNewsItem>>(this, CustomGet("https://www.carmelcollegegouda.nl/vestigingen/antoniuscollege-gouda/infoscherm", (response) =>
            {
                var html = response.downloadHandler.text;

                Regex regex2 = new Regex(@"<div\s+class=""slide-content[^""]*"">([\s\S]*?)<\/div>");
                MatchCollection matches = regex2.Matches(html);

                foreach (Match match1 in matches)
                {
                    string result1 = match1.Groups[1].Value;

                    AntoniusNewsItem item = new AntoniusNewsItem();
                    Regex regex3 = new Regex(@"<h1\s+class=""text-black[^""]*"">(.*?)<\/h1>");
                    Match match2 = regex3.Match(result1);
                    item.Title = match2.Groups[1].Value;

                    Regex regex4 = new Regex(@"<h2\s+class=""h4\s+text-black[^""]*"">(.*?)<\/h2>");
                    Match match3 = regex4.Match(result1);
                    item.SubTitle = match3.Groups[1].Value;

                    Regex regex5 = new Regex(@"<h2>(.*?)<\/h2>");
                    MatchCollection matches2 = regex5.Matches(result1);
                    for (var i = 0; i < matches2.Count; i++)
                    {
                        var match4 = matches2[i];
                        if (i > 0)
                            item.Content += Environment.NewLine;
                        item.Content += match4.Groups[1].Value;
                    }

                    item.Content = HTMLUtils.ReplaceHtmlEntities(item.Content);

                    items.Add(item);
                }

                return items;
            }, _ =>
            {
                AndroidUIToast.ShowToast("Er is een fout opgetreden bij het ophalen van de nieuwsberichten.");
                return null;
            })).result;
    }
    
    public IEnumerator CustomGet(string url, Func<UnityWebRequest, object> callback, Func<UnityWebRequest, object> error = null)
    {
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        while (!www.isDone) yield return null;
            
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(www.error);
            var errored = error.Invoke(www);
            www.Dispose();
            yield return null;
        }

        var returned = callback.Invoke(www);
        www.Dispose();
        yield return returned;
    }


    public class AntoniusNewsItem
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Content { get; set; }
    }
}

public static class HTMLUtils
{
    public static string StripHTML(string input)
    {
        return Regex.Replace(input, "<.*?>", String.Empty);
    }

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
            {"&yuml;", 'ÿ'},
        };

        var regex = new Regex("(&[A-Za-z]+;)");

        var matches = regex.Matches(htmlText ?? "");

        foreach (Match match in matches)
        {
            if (entityMap.TryGetValue(match.Value, out char replacementChar))
            {
                htmlText = htmlText.Replace(match.Value, replacementChar.ToString());
            }
        }

        return htmlText ?? "";
    }
}