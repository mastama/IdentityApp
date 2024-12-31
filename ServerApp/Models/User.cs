using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ServerApp.Models;

[Table("tbl_users")]
public class User : IdentityUser
{
    [Required]
    [StringLength(255)]
    public string? FirstName { get; set; }
    [Required]
    [StringLength(255)]
    public string? LastName { get; set; }
    public DateTime DateCreated { get; set; }
}