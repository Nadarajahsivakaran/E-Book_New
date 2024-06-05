using System.ComponentModel.DataAnnotations;

namespace E_Book.Models
{
    public class RegisterDTO
    {
        public required string FirstName { get; set; }
        public required string Email { get; set; }

        [StringLength(100, ErrorMessage = "Passwords must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Passwords must have at least one non alphanumeric character, one digit ('0'-'9'), and one uppercase ('A'-'Z').")]
        public required string Password { get; set; }

        public required string Role { get; set; }
    }

   
}
