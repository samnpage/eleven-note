using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ElevenNote.Data.Entities;

public class UserEntity : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }
}
