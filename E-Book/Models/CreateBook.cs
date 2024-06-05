namespace E_Book.Models
{
    public class CreateBook
    {
        public int Id { get; set; } = 0;
        public required string Title { get; set; }

        public required string Author { get; set; }

        public IFormFile? ImagePath { get; set; }

        public string? Description { get; set; }

        public required double Price { get; set; }

        public required DateTime PublishedDate { get; set; }

        public int NoOfCopies { get; set; } = 1;
    }
}
