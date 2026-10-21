using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class ResultFormViewModel
{
    public int ResultID { get; set; }

    [Required]
    public int RegistrationID { get; set; }

    public int EventID { get; set; }
    public string ParticipantName { get; set; } = string.Empty;

    [Required]
    public TimeSpan FinishTime { get; set; }

    public TimeSpan? ChipTime { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int OverallPosition { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int CategoryPosition { get; set; }

    [Required]
    public string ResultStatus { get; set; } = "OFFICIAL";
}

public class ResultRowViewModel
{
    public EnrolmentViewModel Enrolment { get; set; } = new();
    public ResultViewModel? Result { get; set; }
}

public class EventResultsViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public List<ResultRowViewModel> Rows { get; set; } = new();
}