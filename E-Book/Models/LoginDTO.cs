namespace E_Book.Models
{
    public class LoginDTO
    {
        public required string UserName { get; set; } = string.Empty;

        public required string Password { get; set; } = string.Empty;
    }
}
