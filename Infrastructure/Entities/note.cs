using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class note
{
    [Key] public string id { get; set; }
    public string notebookId { get; set; }
    public DateTime lastEdit { get; set; }
    public string titel { get; set; }
    public string omschrijving { get; set; }
    public string paragraphs { get; set; }
    public string tags { get; set; }
}