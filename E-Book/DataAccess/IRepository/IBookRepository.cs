using E_Book.Models;
using System.Linq.Expressions;

namespace E_Book.DataAccess.IRepository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAll(string? includeProperties = "", string search="");
        Task Create(Book book);
        Task<Book> GetData(Expression<Func<Book, bool>> predicate, string? includeProperties = "");
        Task Update(Book book);
        Task AddFeedBack(string userId,string feedback,int bookId);
    }
}
