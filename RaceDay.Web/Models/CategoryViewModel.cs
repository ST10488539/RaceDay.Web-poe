namespace RaceDay.Web.Models;

public class CategoryViewModel
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public int MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? Description { get; set; }
}