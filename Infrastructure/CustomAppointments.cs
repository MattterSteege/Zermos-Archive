using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Context;
using Infrastructure.Entities;

namespace Infrastructure
{
    public class CustomAppointments
    {
        private readonly ZermosContext _context;

        public CustomAppointments(ZermosContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Voeg een afspraak toe voor een gebruiker
        /// </summary>
        /// <param name="userEmail">The email of the user</param>
        /// <param name="date">The date of the appointment</param>
        /// <param name="description">The description of the appointment</param>
        /// <param name="location">The location of the appointment</param>
        /// <exception cref="Exception">Thrown when the user does not exist, the date is in the past, the description is null or empty, or the location is null or empty</exception>
        /// <returns></returns>
        public async Task AddAppointmentAsync(DateTime start, DateTime end, string appointmentType, string subject, string description, string location, string userEmail)
        {
            custom_appointment appointment = new()
            {
                start = start,
                end = end,
                appointmentType = appointmentType,
                subject = subject,
                location = location,
                description = description,
                UserEmail = userEmail,
            };
            
            await AddAppointmentAsync(userEmail, appointment);
        }
        
        /// <summary>
        /// Voeg een afspraak toe voor een gebruiker
        /// </summary>
        /// <param name="userEmail">The email of the user</param>
        /// <param name="appointment">The appointment to add</param>
        /// <exception cref="Exception">Thrown when the user does not exist, the date is in the past, the description is null or empty, or the location is null or empty</exception>
        /// <returns></returns>
        public async Task AddAppointmentAsync(string userEmail, custom_appointment appointment)
        {
            // Valideer of de gebruiker bestaat
            var user = await _context.users.FindAsync(userEmail);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            
            appointment.UserEmail = user.email;

            // Voeg de afspraak toe aan de database via de DbSet
            _context.custom_appointments.Add(appointment);

            // Sla de wijzigingen op in de database
            await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// Haal alle afspraken op voor een gebruiker binnen een bepaalde periode
        /// </summary>
        /// <param name="userEmail">The email of the user</param>
        /// <param name="startDate">The start date of the period (inclusive)</param>
        /// <param name="endDate">The end date of the period (inclusive)</param>
        /// <returns>A list of appointments</returns>
        public async Task<List<custom_appointment>> GetAppointmentsForUserAsync(string userEmail, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date; // Set the time to 00:00:00
            endDate = endDate.Date.AddDays(1).AddSeconds(-1); // Set the time to 23:59:59
            
            return await _context.custom_appointments
                .Where(appointment => appointment.UserEmail == userEmail && appointment.start >= startDate && appointment.end <= endDate)
                .ToListAsync();
        }
        
        /// <summary>
        /// Get all appointments for a user
        /// </summary>
        /// <param name="userEmail">The email of the user</param>
        /// <returns>A list of appointments</returns>
        public async Task<List<custom_appointment>> GetAppointmentsForUserAsync(string userEmail)
        {
            return await _context.custom_appointments
                .Where(appointment => appointment.UserEmail == userEmail)
                .ToListAsync();
        }

        /// <summary>
        /// Get an appointment by its ID
        /// </summary>
        /// <param name="id">The ID of the appointment</param>
        /// <returns>The appointment</returns>
        public async Task<int> DeleteAppointmentAsync(string zermosEmail, int id)
        {
            var appointment = await _context.custom_appointments.FindAsync(id);
            if (appointment == null)
            {
                //throw new Exception("Appointment not found");
                return 404;
            }

            if (appointment.UserEmail != zermosEmail)
            {
                //throw new Exception("User does not have permission to delete this appointment");
                return 403;
            }

            _context.custom_appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return 200;
        }
    }
}