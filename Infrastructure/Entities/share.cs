using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class share
{
    [Key] public string key { get; set; }
    public string email { get; set; }
    public string value { get; set; }
    public string page { get; set; }
    public DateTime expires_at { get; set; }
    public int max_uses { get; set; }
    #if DEBUG
    [NotMapped] public string url => $"https://192.168.178.22:5001" + page + "?token=" + key;
    #elif RELEASE
    [NotMapped] public string url => $"https://zermos.kronk.tech" + page + "?token=" + key;
    #endif
}