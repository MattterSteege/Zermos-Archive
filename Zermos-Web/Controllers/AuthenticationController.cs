using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zermos_Web.Models;
using Zermos_Web.Models.Requirements;
using Zermos_Web.Utilities;


namespace Zermos_Web.Controllers;

public class AuthenticationController : Controller
{
    private readonly Users _user;
    private readonly Shares _share;
    private readonly ILogger<BaseController> _logger;

    public AuthenticationController(Users user, Shares share, ILogger<BaseController> logger)
    {
        _user = user;
        _share = share;
        _logger = logger;
    }
    
#region Microsoft Login

#if (RELEASE && !BETA)
    string redirectUrl = "https://zermos.kronk.tech/Login/Callback";
#elif (RELEASE && BETA)
    string redirectUrl = "https://zermos-beta.kronk.tech/Login/Callback";
#else
    string redirectUrl = "https://localhost:5001/Login/Callback";
#endif
    
    const string clientId = "REDACTED_MS_CLIENT_ID";
    const string clientSecret = "REDACTED_MS_CLIENT_SECRET";

    [ZermosPage]
    [Route("/Login")]
    public async Task<IActionResult> Login()
    {
        
        string redirect = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=" +
                          clientId + "&response_type=code&prompt=select_account&redirect_uri=" + redirectUrl +
                          "&response_mode=query&scope=User.Read&state=" + TokenUtils.RandomString();

         return PartialView("Login", new loginModel {email = redirect, code = 0});
    }

    [ZermosPage]
    [Route("/Login/Callback")]
    public async Task<IActionResult> LoginCallback(string code, string state, string session_state)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://login.microsoftonline.com/common/oauth2/v2.0/token");
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("client_id", clientId));
        collection.Add(new("scope", "User.Read"));
        collection.Add(new("code", code));
        collection.Add(new("redirect_uri", redirectUrl));
        collection.Add(new("grant_type", "authorization_code"));
        collection.Add(new("client_secret", clientSecret));
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Log(LogLevel.Error, response.StatusCode + " " + response.ReasonPhrase + " " + await response.Content.ReadAsStringAsync());
            return VerificationFailed(4);
        }

        var responseString = await response.Content.ReadAsStringAsync();

        var auth = JsonConvert.DeserializeObject<MicrosoftAuthenticationModel>(responseString);

        request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", "Bearer " + auth.access_token);

        response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            return VerificationFailed(23);
        }

        var teamsUser = JsonConvert.DeserializeObject<MicrosoftUserModel>(await response.Content.ReadAsStringAsync());
        
        return await VerificationSuccess(teamsUser.mail, null);
    }
    #endregion

    [NonAction]
    private async Task<IActionResult> VerificationSuccess(string email, string ReturnUrl, bool OneSessionLogin = false)
    {
        bool newUser = false;
        
        var claims = new List<Claim>
        {
            new("email", email),
            new("role", "user"),
        };

        var user = await _user.GetUserAsync(email);
        
        if (user == null)
        {
            user = new user
            {
                email = email,
                theme = "light"
            };
            await _user.UpdateUserAsync(email, user);
            newUser = true;
        }

        HttpContext.Response.Cookies.Append("theme", user.theme ?? "light");

        await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

        _logger.Log(LogLevel.Information,$"User {email} logged in");
        
        if (newUser)
        {
            return RedirectToAction("EersteKeer", "Main");
        }

        if (string.IsNullOrEmpty(ReturnUrl))
        {
            return PartialView("Login", new loginModel {email = email, code = 13});
        }

        return Redirect(ReturnUrl);
    }

    [NonAction]
    private IActionResult VerificationFailed(int code)
    {
        switch (code)
        {
            case 21:
                _logger.Log(LogLevel.Warning, "User not found in database, returned code 21");
                return PartialView("Login", new loginModel {code = 21});
            case 22:
                _logger.Log(LogLevel.Warning, "Token expired, returned code 22");
                return PartialView("Login", new loginModel {code = 22});
            case 23:
                _logger.Log(LogLevel.Warning, "Token incorrect, returned code 23");
                return PartialView("Login", new loginModel {code = 23});
            case 24:
                _logger.Log(LogLevel.Warning, "User already verified, returned code 24");
                return PartialView("Login", new loginModel {code = 24});
            default:
                _logger.Log(LogLevel.Warning, "Unknown error, returned code 4");
                return PartialView("Login", new loginModel {code = 4});
        }
    }

    [ZermosPage]
    [Route("/Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _logger.Log(LogLevel.Information, $"User {User.FindFirstValue("email")} logged out");
        return PartialView("Login", new loginModel {code = 3});
    }

    #if DEBUG
    [ZermosPage]
    [Route("/LoginPage")]
    public IActionResult TestPages(int code = 0)
    {
        return PartialView("Login", new loginModel {code = code});
    }
    
    [ZermosPage]
    [Route("/LoginAs")]
    public async Task<IActionResult> LoginAs(string user)
    {
        if (User.FindFirstValue("email") != "58373@ccg-leerling.nl")
        {
            return PartialView("Login", new loginModel {code = 4});
        }
        
        if (!string.IsNullOrEmpty(user))
            return await VerificationSuccess(user, null);

        return PartialView((await _user.GetUsersAsync()));
    }
    #endif

    public class loginModel
    {
        public string email { get; set; }
        public int code { get; set; }
    }
}