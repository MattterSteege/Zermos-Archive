using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Zermos_Web.Controllers;

public class AfsprakenController : BaseController
{
    public AfsprakenController(Users user, Shares share, CustomAppointments customCustomAppointment, ILogger<BaseController> logger, IHttpClientFactory httpClientFactory) : base(user, share, customCustomAppointment, logger, httpClientFactory) { }

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
            await AddCustomAppointment(custom_appointment);
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

        var appointments = await GetCustomAppointmentsForUser(startDate.Value, endDate.Value);

        //remove id, email and user from the appointments
        foreach (var appointment in appointments)
        {
            appointment.email = null;
            appointment.description = appointment.description == "null" ? null : appointment.description;
        }

        return Ok(appointments);
    }
    
    [Authorize]
    [HttpDelete("/Api/Afspraken/Verwijder")]
    public async Task<IActionResult> DeleteCustomAppointment(int id)
    {
        if (id == 0 || id < 0)
            return BadRequest(new {error = "Id is set to an invalid value, dunno how you did that"});
        
        int code = await DeleteCustomAppointmentForUser(id);
        if (code == 404)
            return NotFound(new {error = "Appointment not found"});
        if (code == 403)
            return StatusCode(403, new {error = "User does not have permission to delete this appointment"});
        
        return Ok(new {message = "Appointment deleted successfully"});
    }
}