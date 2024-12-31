using System.ComponentModel.DataAnnotations;

namespace ServerApp.DTOs.Account;

public class RegisterDto
{
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between {2} and {1} characters")]
    public string? FirstName { get; set; }
    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Email must be between {2} and {1} characters")]
    public string? LastName { get; set; }
    [Required]
    [EmailAddress]
    [RegularExpression("^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\\.)+(?:[a-zA-Z]{2}|aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel)$", ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between {2} and {1} characters")]
    public string? Password { get; set; }
    [Required]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between {2} and {1} characters")]
    [RegularExpression("^(?:\\+62|62|0)8[1-9][0-9]{6,11}$", ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }
}