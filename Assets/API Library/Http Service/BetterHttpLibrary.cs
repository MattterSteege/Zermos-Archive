using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UnityEngine;

public class BetterHttpLibrary : MonoBehaviour
{
    private HttpClient _client;
    private HttpClientHandler _handler;
    private HttpRequestMessage _request;
    private Dictionary<string, string> _headers;

    public BetterHttpLibrary()
    {
        
        _client = new HttpClient();
        _handler = new HttpClientHandler();
        _request = new HttpRequestMessage();
        _headers = new Dictionary<string, string>();
    }

    #region Request Methods
    public BetterHttpLibrary Get(string url)
    {
        _request.Method = HttpMethod.Get;
        _request.RequestUri = new Uri(url);
        return this;
    }
    
    public BetterHttpLibrary Post(string url)
    {
        _request.Method = HttpMethod.Post;
        _request.RequestUri = new Uri(url);
        return this;
    }
    
    public BetterHttpLibrary Put(string url)
    {
        _request.Method = HttpMethod.Put;
        _request.RequestUri = new Uri(url);
        return this;
    }
    
    public BetterHttpLibrary Delete(string url)
    {
        _request.Method = HttpMethod.Delete;
        _request.RequestUri = new Uri(url);
        return this;
    }
    #endregion
    
    #region Request Header Methods
    public BetterHttpLibrary AddHeader(string key, string value)
    {
        _headers.Add(key, value);
        return this;
    }

    public BetterHttpLibrary AddHeaders(Dictionary<string, string> headers)
    {
        _headers = new Dictionary<string, string>(headers);
        return this;
    }
    #endregion

    #region Request Body Methods
    public BetterHttpLibrary AddBody(string body)
    {
        _request.Content = new StringContent(body);
        return this;
    }
    
    public BetterHttpLibrary AddBody(byte[] body)
    {
        _request.Content = new ByteArrayContent(body);
        return this;
    }
    
    public BetterHttpLibrary AddBody(Dictionary<string, string> body)
    {
        _request.Content = new FormUrlEncodedContent(body);
        return this;
    }
    
    public BetterHttpLibrary AddBody(object body)
    {
        _request.Content = new StringContent(JsonUtility.ToJson(body));
        return this;
    }
    
    public BetterHttpLibrary AddBody<T>(T body)
    {
        _request.Content = new StringContent(JsonUtility.ToJson(body));
        return this;
    }
    
    public BetterHttpLibrary AddBody<T>(T[] body)
    {
        _request.Content = new StringContent(JsonUtility.ToJson(body));
        return this;
    }
    
    public BetterHttpLibrary AddBody<T>(List<T> body)
    {
        _request.Content = new StringContent(JsonUtility.ToJson(body));
        return this;
    }
    
    public BetterHttpLibrary AddBody<T>(Dictionary<string, T> body)
    {
        _request.Content = new StringContent(JsonUtility.ToJson(body));
        return this;
    }
    #endregion

    #region Request Settings Methods
    public BetterHttpLibrary AllowCustomSetting()
    {
        _client = new HttpClient(_handler);
        return this;
    }
    
    public BetterHttpLibrary SetTimeout(int timeout)
    {
        _client.Timeout = TimeSpan.FromSeconds(timeout);
        return this;
    }
    
    public BetterHttpLibrary SetTimeout(TimeSpan timeout)
    {
        _client.Timeout = timeout;
        return this;
    }
    
    public BetterHttpLibrary DisallowRedirects(bool redirect = false)
    {
        _handler.AllowAutoRedirect = redirect;
        return this;
    }
    
    public BetterHttpLibrary SetRedirectLimit(int limit)
    {
        _handler.MaxAutomaticRedirections = limit;
        return this;
    }
    
    public BetterHttpLibrary SetCookie(string cookieName, string cookieValue)
    {
        _handler.CookieContainer.Add(new Uri(_request.RequestUri.GetLeftPart(UriPartial.Authority)), new Cookie(cookieName, cookieValue));
        return this;
    }

    public BetterHttpLibrary SetCookies(Dictionary<string, string> cookies)
    {
        foreach (var cookie in cookies)
        {
            _handler.CookieContainer.Add(new Uri(_request.RequestUri.GetLeftPart(UriPartial.Authority)), new Cookie(cookie.Key, cookie.Value));
        }
        return this;
    }
    #endregion

    #region BetterHttpLibrary
    public BetterHttpLibrary OnSuccess(Action<HttpResponseMessage> callback)
    {
        onSuccess = callback;
        return this;
    }

    public BetterHttpLibrary OnFailure(Action<HttpResponseMessage> callback)
    {
        onFailure = callback;
        return this;
    }
    #endregion
    
    public BetterHttpLibrary SendRequest()
    {
        try
        {
            var response = _client.SendAsync(_request).Result;
            if (response.IsSuccessStatusCode)
            {
                onSuccess?.Invoke(response);
            }
            else
            {
                onFailure?.Invoke(response);
            }
        }
        catch (Exception)
        {
            onFailure?.Invoke(null);
        }
        return this;
    }

    public Action<HttpResponseMessage> onSuccess { get; set; }
    public Action<HttpResponseMessage> onFailure { get; set; }
}