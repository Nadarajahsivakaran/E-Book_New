using System.ComponentModel;

namespace E_Book.Models
{
    public class Report
    {
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        public IEnumerable<Book>? Books { get; set; } = [];

        public int? BookID { get; set; }

        public IEnumerable<UserWithRolesDTO>? Users { get; set; } = [];

        public int? UserID { get; set; }

        public IEnumerable<Order>? Orders { get; set; } = [];
    }
}
