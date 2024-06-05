using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Book.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required int OrderId { get; set; }
        public Order? Order { get; set; } 

        public required int BookId { get; set; }
        public Book? Book { get; set; } 
        
        public required int Quantity { get; set; }

        public bool IsDelete { get; set; } = false;
    }
}
