namespace RaceDay.Web.Models;

public class ResultViewModel
{
    public int ResultID { get; set; }
    public int RegistrationID { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string RaceNumber { get; set; } = string.Empty;
    public TimeSpan FinishTime { get; set; }
    public TimeSpan? ChipTime { get; set; }
    public int OverallPosition { get; set; }
    public int CategoryPosition { get; set; }
    public decimal Pace { get; set; }
    public string ResultStatus { get; set; } = string.Empty;
}