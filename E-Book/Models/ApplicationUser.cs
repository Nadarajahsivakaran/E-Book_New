using Microsoft.AspNetCore.Identity;

namespace E_Book.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public string RoleName { get; set; } =string.Empty;
    }
}
