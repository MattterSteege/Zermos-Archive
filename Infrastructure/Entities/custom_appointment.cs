using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class custom_appointment
{
    [Key] public int Id { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
    public string appointmentType { get; set; }
    public string subject { get; set; }
    public string description { get; set; }
    public string location { get; set; }
    
    // Foreign key to associate with a user
    public string email { get; set; }
    
    // Navigation property
    //[ForeignKey("UserEmail")]
   // public user User { get; set; }
}