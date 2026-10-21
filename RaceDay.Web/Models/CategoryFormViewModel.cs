using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class CategoryFormViewModel
{
    public int CategoryID { get; set; }
    public int EventID { get; set; }

    [Required]
    public string CategoryName { get; set; } = string.Empty;

    [Required]
    public decimal Distance { get; set; }

    [Required]
    public int MinAge { get; set; }

    public int? MaxAge { get; set; }

    public string? Description { get; set; }
}