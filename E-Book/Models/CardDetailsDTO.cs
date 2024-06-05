namespace E_Book.Models
{
    public class CardDetailsDTO
    {
        public int BookId { get; set; }

        public string BookName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public double Quantity { get; set; }

        public double Price { get; set; }
    }
}
