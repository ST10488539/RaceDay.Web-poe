using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class EventFormViewModel
{
    public int EventID { get; set; }

    [Required]
    public string EventName { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime EventDate { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public decimal Distance { get; set; }

    [Required]
    public string EventType { get; set; } = "RUN"; // RUN, WALK, CYCLE

    public string? ImageURL { get; set; }
}