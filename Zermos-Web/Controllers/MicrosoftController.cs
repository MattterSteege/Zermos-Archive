// using System;
// using System.Net.Http;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Infrastructure;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Zermos_Web.Controllers
// {
//     public class MicrosoftController : Controller
//     {
//         private readonly Users _user;
//         private readonly HttpClient _httpClient;
//
//         public MicrosoftController(Users user)
//         {
//             _user = user;
//             _httpClient = new HttpClient
//             {
//                 BaseAddress = new Uri("https://graph.microsoft.com/v1.0/me/drive")
//             };
//         }
//
//         public async Task<IActionResult> Onedrive()
//         {
//             string accessToken = await getAccessToken();
//         }
//
//         private async Task<string> getAccessToken()
//         {
//             string refresh_token = (await _user.GetUserAsync(User.FindFirstValue("email"))).microsoft_refresh_token;
//             var values = new Dictionary<string, string>
//             {
//                 {"client_id", "b3b9c0a0-5b7a-4a1a-8f3e-0b1f4b8b1f9e"},
//                 {"s
//         }
//     }
// }