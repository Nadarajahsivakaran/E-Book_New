using E_Book.Models;

namespace E_Book.DataAccess.IRepository
{
    public interface IAuthRepository
    {
        Task<ServiceResponse> Register(RegisterDTO register);

        Task<ServiceResponse> Login(LoginDTO login);

        Task<ServiceResponse> Logout();

        Task<ServiceResponse> GetCustomer();

        Task<ServiceResponse> GetCustomerAdmin();

    }

}
