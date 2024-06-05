using System.ComponentModel.DataAnnotations;

namespace E_Book.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public required string UserID { get; set; }
        public ApplicationUser? User { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];
    }
}
