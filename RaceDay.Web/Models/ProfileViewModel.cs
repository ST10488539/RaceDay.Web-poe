using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class ProfileViewModel
{
    public int UserID { get; set; }

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime DateCreated { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }
    public string? EmergencyContact { get; set; }
    public string? MedicalNotes { get; set; }
}