using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace zermos_authentication_service.Controllers
{
    [ApiController]
    [Route("zermelo")]
    public class zermelo : ControllerBase
    {
        public string Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Zermos service Zermelo API\n");
            sb.Append($"The base url for the endpoints below is: {Request.Scheme}://{Request.Host}{Request.PathBase}/{Request.Path.ToString().Replace("/",  "")}\n");
            sb.Append("The current endpoints for this service are:\n\n");
            
            foreach (var method in typeof(zermelo).GetMethods())
            {
                if (method.GetCustomAttributes(typeof(HttpGetAttribute), true).Length > 0)
                {
                    // find the base url of the asp.net api
                    sb.Append($"    - /{method.GetCustomAttribute<HttpGetAttribute>()?.Template}");

                    if (method.GetParameters().Length > 0)
                    {
                        sb.Append(" (");

                        foreach (var parameter in method.GetParameters())
                        {
                            sb.Append($"{parameter.ParameterType.Name} {parameter.Name}");

                            if (parameter != method.GetParameters()[method.GetParameters().Length - 1])
                            {
                                sb.Append(", ");
                            }
                        }

                        sb.Append(")\n");
                    }
                }
            }

            return sb.ToString();
        }
        
        [HttpGet("dagrooster")]
        public string GetDagrooster(string access_token, string student, string weeknummer)
        {
            /*"GET", "https://ccg.zportal.nl/api/v3/liveschedule?access_token=" + access_token + "&student=" + student + "&week=2023" + weekNumber*/
            
            //HttpRequest request = new HttpRequest();
            //request.Method = "GET";
            
            
            
        }
        
    }
}