using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Newtonsoft.Json;
using Zermos_Web.Models.zermelo;
using Zermos_Web.Models.Zermelo;
using Zermos_Web.Utilities;
using Appointment = Zermos_Web.Models.zermelo.Appointment;
using Response = Zermos_Web.Models.zermelo.Response;

namespace Zermos_Web.APIs
{
    /// <summary>
    /// Represents a class for interacting with the Zermelo API to retrieve schedule information.
    /// </summary>
    public class ZermeloApi
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZermeloApi"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making API requests.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided HttpClient is null.</exception>
        public ZermeloApi(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        
        /// <summary>
        /// Retrieves the Zermelo schedule for a specified user, year, and week.
        /// </summary>
        /// <param name="user">The user for whom the schedule is requested.</param>
        /// <param name="year">The year for which the schedule is requested.</param>
        /// <param name="week">The week for which the schedule is requested.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation with the Zermelo schedule model.</returns>
        /// <exception cref="ArgumentNullException">Thrown if user, access token, school ID, year, or week is null or empty.</exception>
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
                return EmptyModel();

            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
            return zermeloRoosterModel;
        }
        
        /// <summary>
        /// Enrolls the specified user into a lesson asynchronously.
        /// </summary>
        /// <param name="user">The user to be enrolled.</param>
        /// <param name="post">The URL endpoint for the lesson enrollment.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result is a boolean indicating whether the enrollment was successful.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when either the user or post parameter is null or empty.</exception>
        public async Task<bool> EnrollIntoLessonAsync(user user, string post)
        {
            if (user.zermelo_access_token.IsNullOrEmpty()) throw new ArgumentNullException(nameof(user));
            if (post.IsNullOrEmpty()) throw new ArgumentNullException();

            string baseUrl = "https://ccg.zportal.nl" + post + $"?access_token={user.zermelo_access_token}";

            var response = await _httpClient.GetAsync(baseUrl);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<SimpleZermeloRoosterModel> getRoosterFromStartAndEnd(user user, DateTime startUnix, DateTime endUnix)
        {
            var baseUrl = $"https://ccg.zportal.nl/api/v3/appointments" +
                          $"?user={user.school_id}" +
                          $"&access_token={user.zermelo_access_token}" +
                          $"&start={startUnix.ToUnixTime()}" +
                          $"&end={endUnix.ToUnixTime()}" +
                          $"&fields=subjects,groups,locations,teachers,cancelled,type,start,end";
            
            var response = await _httpClient.GetAsync(baseUrl);

            if (!response.IsSuccessStatusCode)
            {
                var end = new SimpleZermeloRoosterModel
                {
                    response = new Models.Zermelo.Response
                    {
                        data = new List<Models.Zermelo.Appointment>()
                    }
                };
                end.response.data.Add(new Models.Zermelo.Appointment());
                return end;
            }
            
            return JsonConvert.DeserializeObject<SimpleZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
        }

        // Private method to create an empty ZermeloRoosterModel for error handling.
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
