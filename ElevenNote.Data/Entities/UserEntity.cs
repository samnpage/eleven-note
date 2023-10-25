using System.ComponentModel.DataAnnotations;

namespace ElevenNote.Data.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }
}
