namespace E_Book.Models
{
    public class ServiceResponse
    {
        public bool IsSuccess { get; set; } = true;

        public object? Result { get; set; } = null;
    }
}
