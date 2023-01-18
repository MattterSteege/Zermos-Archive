using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ParseQueryString
{
    public static NameValueCollection ParseQuery(string query)
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
}
