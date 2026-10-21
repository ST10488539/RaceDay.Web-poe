namespace RaceDay.Web.Models;

public class EventViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Distance { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ImageURL { get; set; }
    public int OrganiserID { get; set; }
    public string OrganiserName { get; set; } = string.Empty;
}