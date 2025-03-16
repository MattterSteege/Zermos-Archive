using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Entities;
using Newtonsoft.Json;
using Zermos_Web.Models.zermelo;
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

            if (week == "53")
            {
                week = "01";
                int _year = int.Parse(year);
                _year++;
                year = _year.ToString();
            } 

            var baseUrl = $"https://{user.zermelo_school_abbr}.zportal.nl/api/v3/liveschedule" +
                          $"?access_token={user.zermelo_access_token}" +
                          $"&student={user.school_id}" +
                          $"&week={year}{week}";

            var response = await _httpClient.GetAsync(baseUrl);

            if (!response.IsSuccessStatusCode)
            {
                return EmptyModel();
            }

            var timestamps = user.zermelo_timestamps ?? "08:00-17:00";
            timestamps = (timestamps == "-" ? "08:00-17:00" : timestamps);
            var start = timestamps.Split('-')[0].Split(':');
            var end = timestamps.Split('-')[1].Split(':');
            var secondsStart = int.Parse(start[0]) * 3600 + int.Parse(start[1]) * 60;
            var secondsEnd = int.Parse(end[0]) * 3600 + int.Parse(end[1]) * 60;
            
            //"[28800, 61200]" -> 08:00 - 17:00 in seconds

            var zermeloRoosterModel = JsonConvert.DeserializeObject<ZermeloRoosterModel>(await response.Content.ReadAsStringAsync());
            zermeloRoosterModel.MondayOfAppointmentsWeek = DateTimeUtils.GetMondayOfWeekAndYear(week, year);
            zermeloRoosterModel.timeStamps = new List<int> {secondsStart, secondsEnd}; //so divide by 60 to get minutes and divide by 3600 to get hours
            zermeloRoosterModel.roosterOrigin = "zermelo";
            zermeloRoosterModel.tropenRoosterDuration = int.Parse(user.tropen_rooster_time ?? "0");
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

            string baseUrl = $"https://{user.zermelo_school_abbr}.zportal.nl" + post + $"?access_token={user.zermelo_access_token}";

            var response = await _httpClient.GetAsync(baseUrl);
            
            return response.IsSuccessStatusCode;
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
