using System;
using System.Globalization;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers;

public class AfsprakenController : BaseController
{
    public AfsprakenController(Users user, Shares share, CustomAppointments customCustomAppointment,
        ILogger<BaseController> logger) : base(user, share, customCustomAppointment, logger)
    {
    }

    [Authorize]
    [HttpPost("/Api/Afspraken/Nieuw")]
    public async Task<IActionResult> AddCustomAppointment()
    {
        if (string.IsNullOrEmpty(Request.Form["start"]) || string.IsNullOrEmpty(Request.Form["end"]) ||
            string.IsNullOrEmpty(Request.Form["appointmentType"]) || string.IsNullOrEmpty(Request.Form["subject"]) ||
            string.IsNullOrEmpty(Request.Form["location"]) || string.IsNullOrEmpty(Request.Form["description"]))
            return BadRequest(new
            {
                error =
                    "One or more fields are null or empty, The fields needed are: start, end, appointmentType, subject, location, description"
            });

        string format = "dd-MM-yyyy HH:mm";
        DateTime start = DateTime.ParseExact(Request.Form["start"], format, CultureInfo.InvariantCulture);
        DateTime end = DateTime.ParseExact(Request.Form["end"], format, CultureInfo.InvariantCulture);

        var custom_appointment = new custom_appointment
        {
            start = start,
            end = end,
            appointmentType = Request.Form["appointmentType"],
            subject = Request.Form["subject"],
            location = Request.Form["location"],
            description = Request.Form["description"]
        };


        if (custom_appointment.start > custom_appointment.end)
            return BadRequest(new {error = "Start date is after end date"});
        if (custom_appointment.end < DateTime.Now)
            return BadRequest(new {error = "Date is in the past"});
        if (string.IsNullOrEmpty(custom_appointment.description))
            return BadRequest(new {error = "Description is null or empty"});

        try
        {
            await CustomCustomAppointment.AddAppointmentAsync(ZermosEmail, custom_appointment);
            return Ok(new {message = "Appointment added successfully"});
        }
        catch (Exception ex)
        {
            return BadRequest(new {error = ex.Message});
        }
    }

    [Authorize]
    [HttpGet("/Api/Afspraken/AlleAfspraken")]
    public async Task<IActionResult> GetCustomAppointments(string start, string end, string jaarWeek)
    {
        DateTime? startDate = null;
        DateTime? endDate = null;

        if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
        {
            //dd-MM-yyyy
            string format = "dd-MM-yyyy";
            if (!string.IsNullOrEmpty(start))
                startDate = DateTime.ParseExact(start, format, null);
            if (!string.IsNullOrEmpty(end))
                endDate = DateTime.ParseExact(end, format, null);
        }
        else if (!string.IsNullOrEmpty(jaarWeek))
        {
            //yyyyWW
            startDate = DateTimeUtils.GetMondayOfWeekAndYear(jaarWeek.Substring(4, 2),
                jaarWeek.Substring(0, 4)); //Monday
            endDate = startDate.Value.AddDays(6); //Sunday
        }
        else
        {
            return BadRequest(new {error = "Either start and end or jaarWeek must be filled"});
        }


        if (startDate == null)
            startDate = DateTime.MinValue;
        if (endDate == null)
            endDate = DateTime.MaxValue;

        var appointments =
            await CustomCustomAppointment.GetAppointmentsForUserAsync(ZermosEmail, (DateTime) startDate,
                (DateTime) endDate);

        //remove id, useremail and user from the appointments
        foreach (var appointment in appointments)
        {
            appointment.Id = -1;
            appointment.UserEmail = null;
            appointment.User = null;
        }

        return Ok(appointments);
    }
}