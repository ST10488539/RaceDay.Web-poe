namespace RaceDay.Web.Models;

public class OrganiserDashboardViewModel
{
    public List<EventViewModel> Events { get; set; } = new();
    public int TotalEvents => Events.Count;
    public Dictionary<int, int> EnrolmentCountsByEvent { get; set; } = new();
    public int TotalEnrolments => EnrolmentCountsByEvent.Values.Sum();
}