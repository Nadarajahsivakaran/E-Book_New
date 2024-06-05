using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Book.Models
{
    public class BookFeedBack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required int BookId { get; set; }
        public Book? Book { get; set; }

        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public required string FeedBack { get; set; }
    }
}
