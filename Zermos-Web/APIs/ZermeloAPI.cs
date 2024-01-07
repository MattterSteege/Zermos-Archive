using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Newtonsoft.Json;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Utilities;

namespace Zermos_Web.APIs
{
    public class ZermeloApi
    {
        private readonly HttpClient _httpClient;

        public ZermeloApi(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ZermeloRoosterModel> GetRoosterAsync(user user, string year, string week)
        {
            if (user.zermelo_access_token.IsNullOrEmpty() || user.school_id.IsNullOrEmpty()) throw new ArgumentNullException(nameof(user));
            if (year.IsNullOrEmpty() || week.IsNullOrEmpty()) throw new ArgumentNullException();

            var baseUrl = $"https://ccg.zportal.nl/api/v3/liveschedule" +
                          $"?access_token={user.zermelo_access_token}" +
                          $"&student={user.school_id}" +
                          $"&week={year}{week}";

            var response = await _httpClient.GetAsync(baseUrl);

            if (!response.IsSuccessStatusCode)
            {
                return EmptyModel();
            }

            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
            return zermeloRoosterModel;
        }

        private ZermeloRoosterModel EmptyModel()
        {
            return new ZermeloRoosterModel
            {
                response = new Response
                {
                    data = new List<Items>
                    {
                        new()
                        {
                            appointments = new List<Appointment>()
                        }
                    }
                }
            };
        }
    }
}
