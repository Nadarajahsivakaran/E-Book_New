using E_Book.Models;

namespace E_Book.DataAccess.IRepository
{
    public interface IOrderRepository
    {
        Task<ServiceResponse> AddToCart(int BookId, int Quantity, string userId);
        Task<Order> GetCart(string userId);
        Task<ServiceResponse> DropCart(int id);
        Task<ServiceResponse> GetAllOrder();
        Task<ServiceResponse> GetAllOrderReport(string? userId);

        Task<ServiceResponse> Report();
    }
}
