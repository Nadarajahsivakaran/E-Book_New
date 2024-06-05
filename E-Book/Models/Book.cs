using System.ComponentModel.DataAnnotations;

namespace E_Book.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Author { get; set; }

        public string? Image { get; set; }

        public string? Description { get; set; }

        public required double Price { get; set; }

        public required DateTime PublishedDate { get; set; }

        public int  NoOfCopies { get; set; } = 1;

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];

        public ICollection<BookFeedBack> BookFeedBacks { get; set; } = [];
    }
}
