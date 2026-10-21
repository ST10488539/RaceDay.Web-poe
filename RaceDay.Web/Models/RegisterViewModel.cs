using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models;

public class RegisterViewModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Participant"; // "Organiser" or "Participant"

    public string? Phone { get; set; }

    // Participant-only fields
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; } // MALE, FEMALE, OTHER

    public string? EmergencyContact { get; set; }

    public string? MedicalNotes { get; set; }
}