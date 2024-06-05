using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace E_Book.DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ServiceResponse _serviceResponse;

        public OrderRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _serviceResponse = new ServiceResponse();
        }
        public async Task<ServiceResponse> AddToCart(int BookId, int Quantity, string userId)
        {
            try
            {
                Book? book = _dbContext.Books.FirstOrDefault(b => b.Id == BookId);

                int ordersCount = _dbContext.OrderDetail
                              .Where(od => od.BookId == BookId && od.IsDelete == false)
                              .Sum(od => od.Quantity);
                if ((book.NoOfCopies-ordersCount) > Quantity)
                {

                    Order? order = _dbContext.Order.FirstOrDefault(o => o.UserID == userId);

                    if (order == null)
                    {
                        // Create a new order and add the book to it
                        Order newOrder = new Order
                        {
                            UserID = userId,
                        };
                        await _dbContext.Order.AddAsync(newOrder);
                        await _dbContext.SaveChangesAsync();

                        OrderDetail newOrderDetails = new OrderDetail
                        {
                            OrderId = newOrder.Id,
                            BookId = BookId,
                            Quantity = Quantity
                        };
                        await _dbContext.OrderDetail.AddAsync(newOrderDetails);

                    }
                    else
                    {
                        OrderDetail? IsBook = _dbContext.OrderDetail.FirstOrDefault(b => b.BookId == BookId && b.OrderId== order.Id);

                        if (IsBook == null)
                        {
                            OrderDetail newOrderDetails = new OrderDetail
                            {
                                OrderId = order.Id,
                                BookId = BookId,
                                Quantity = Quantity
                            };
                            await _dbContext.OrderDetail.AddAsync(newOrderDetails);
                        }
                        else
                        {
                            IsBook.Quantity = IsBook.Quantity + Quantity;
                            IsBook.IsDelete = false;
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    _serviceResponse.IsSuccess = true;
                    return _serviceResponse;
                }
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = "The book is not avilable";
                return _serviceResponse;

            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex;
                return _serviceResponse;
            }
        }

        public async Task<ServiceResponse> DropCart(int id)
        {
            try
            {
                OrderDetail? orderDetail = await _dbContext.OrderDetail.FirstOrDefaultAsync(o => o.Id == id);
                orderDetail.IsDelete = true;
                orderDetail.Quantity = 0;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex;
            }
            return _serviceResponse;
        }

        public async Task<ServiceResponse> GetAllOrder()
        {
            try
            {

                IEnumerable<Order> orders = await _dbContext.Order
                                    .Include(order => order.OrderDetails)
                                        .ThenInclude(orderDetail => orderDetail.Book)
                                    .Include(order => order.User)
                                    .Select(order => new Order
                                    {
                                        Id = order.Id,
                                        UserID = order.UserID,
                                        User = order.User,
                                        Date = order.Date,
                                        OrderDetails = order.OrderDetails
                                                .Where(orderDetail => !orderDetail.IsDelete)
                                                .Select(orderDetail => new OrderDetail
                                                {
                                                    Id = orderDetail.Id,
                                                    OrderId = orderDetail.OrderId,
                                                    BookId = orderDetail.BookId,
                                                    Book = orderDetail.Book,
                                                    Quantity = orderDetail.Quantity,
                                                    IsDelete = orderDetail.IsDelete
                                                }).ToList()
                                    })
                                        .ToListAsync();


                _serviceResponse.Result = orders;
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex;
            }
            return _serviceResponse;
        }

        public async Task<ServiceResponse> GetAllOrderReport(string? userId)
        {
            ServiceResponse _serviceResponse = new ServiceResponse();

            try
            {
                // Define a queryable for orders
                IQueryable<Order> query = _dbContext.Order
                                                     .Include(order => order.OrderDetails)
                                                         .ThenInclude(orderDetail => orderDetail.Book)
                                                     .Include(order => order.User);

                // If a userId is provided, filter the orders by userId
                if (!string.IsNullOrEmpty(userId) && userId != "0")
                {
                    query = query.Where(order => order.UserID == userId);
                }

                // Execute the query and project the result
                IEnumerable<Order> orders = await query
                                                 .Select(order => new Order
                                                 {
                                                     Id = order.Id,
                                                     UserID = order.UserID,
                                                     User = order.User,
                                                     Date = order.Date,
                                                     OrderDetails = order.OrderDetails
                                                                 .Where(orderDetail => !orderDetail.IsDelete)
                                                                 .Select(orderDetail => new OrderDetail
                                                                 {
                                                                     Id = orderDetail.Id,
                                                                     OrderId = orderDetail.OrderId,
                                                                     BookId = orderDetail.BookId,
                                                                     Book = orderDetail.Book,
                                                                     Quantity = orderDetail.Quantity,
                                                                     IsDelete = orderDetail.IsDelete
                                                                 }).ToList()
                                                 })
                                                 .ToListAsync();

                _serviceResponse.Result = orders;
                _serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex.ToString();
            }

            return _serviceResponse;
        }


        public async Task<Order> GetCart(string userId)
        {

            try
            {
                Order? order = await _dbContext.Order
                     .Include(o => o.OrderDetails)
                         .ThenInclude(od => od.Book)
                     .FirstOrDefaultAsync(o => o.UserID == userId && o.OrderDetails.Any(od => !od.IsDelete));

                if (order != null)
                {
                    order.OrderDetails = order.OrderDetails.Where(od => !od.IsDelete).ToList();
                }


                return order;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse> Report()
        {
            try
            {
                _serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex;
            }
            return _serviceResponse;
        }
    }
}
