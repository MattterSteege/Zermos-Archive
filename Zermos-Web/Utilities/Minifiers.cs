using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Zermos_Web.Utilities;

public static class Minifiers
{
    public static List<string> GetScripts(string html)
    {
        //regex <script\b[^>]*>([\s\S]*?)<\/script>
        //if match contains src, skip
        //if not, add to list
        //return list
        
        Regex regex = new Regex(@"<script\b[^>]*>([\s\S]*?)<\/script>");
        MatchCollection matches = regex.Matches(html);
        List<string> scripts = new();
        
        foreach (Match match in matches)
        {
            if (match.Value.Contains("src"))
                continue;
            
            scripts.Add(match.Value);
        }
        
        return scripts;
    }
    
    public static string MinifyJavaScript(string inputJS)
    {
        // Remove comments
        inputJS = Regex.Replace(inputJS, @"/\*.*?\*/", "", RegexOptions.Singleline);
        inputJS = Regex.Replace(inputJS, @"//[^\n]*", "");

        // Remove white spaces
        inputJS = Regex.Replace(inputJS, @"\s+", " ");

        return inputJS.Trim();
    }
}