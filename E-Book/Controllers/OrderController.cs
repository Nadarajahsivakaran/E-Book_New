using Azure;
using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace E_Book.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAuthRepository _authRepository;
        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository, IAuthRepository authRepository)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _authRepository = authRepository;
        }

        [HttpGet("BookView/{Id}")]
        public async Task<IActionResult> BookView(int Id)
        {
            Book book = null;
            try
            {
                book = await _bookRepository.GetData(b => b.Id == Id, "BookFeedBacks");
                return View(book);
            }
            catch (Exception)
            {
                return View(book);
            }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int BookId, int Quantity)
        {
            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    ServiceResponse response = await _orderRepository.AddToCart(BookId, Quantity, userId);

                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Successfully Added";
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                    }
                    else
                    {
                        TempData["error"] = response.Result;
                        return Json(new { success = false, message = response.Result });
                    }
                }
                else
                {
                    TempData["error"] = "Please Login First!";
                    return Json(new { success = false, redirectUrl = Url.Action("SignIn", "Auth") });
                }

            }
            catch (Exception)
            {
                return View();
            }
        }


        [HttpGet("Cart")]
        public async Task<IActionResult> Cart(bool viewPage = true)
        {

            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId != null)
                {
                    Order order = await _orderRepository.GetCart(userId);

                    if (order == null || order.OrderDetails == null)
                    {
                        TempData["error"] = "No Books";
                        return RedirectToAction("Index", "Home");
                    }

                    if (viewPage)
                        return View(order);
                    else
                    {
                        if (order != null && order.OrderDetails != null && order.OrderDetails.Count > 0)
                        {
                            int totalQuantity = order.OrderDetails.Sum(od => od.Quantity);
                            return Json(totalQuantity);
                        }

                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return View();
            }

        }

        [HttpGet("DropCart/{Id}")]
        public async Task<IActionResult> DropCart(int id)
        {
            try
            {
                ServiceResponse response = await _orderRepository.DropCart(id);

                if (response.IsSuccess)
                {
                    TempData["success"] = "Successfully Removed";
                    return RedirectToAction("Cart");
                }
                else
                    TempData["error"] = response.Result;


            }
            catch (Exception)
            {
                TempData["error"] = "Something Wrong";
            }
            return View();
        }

        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {
                ServiceResponse response = await _orderRepository.GetAllOrder();
                return View(response.Result);
            }
            catch (Exception)
            {
            }
            return View();
        }

        [HttpGet("Report")]
        public async Task<IActionResult> Report(string? userId, int? bookId, DateTime? startDate, DateTime? endDate)
        {
            Report report = new();
            try
            {
                IEnumerable<Book> books = await _bookRepository.GetAll();
                report.Books = books;

                ServiceResponse cusResponse = await _authRepository.GetCustomerAdmin();
                report.Users = cusResponse.Result as IEnumerable<UserWithRolesDTO>;

                ServiceResponse ordResponse = await _orderRepository.GetAllOrderReport(userId);

                if (ordResponse.Result is IEnumerable<Order> orders)
                {
                    if (!string.IsNullOrEmpty(userId) && userId != "0")
                        orders = orders.Where(o => o.UserID == userId);

                    if (bookId.HasValue && bookId.Value > 0)
                    {
                        orders = orders.Select(o =>
                        {
                            o.OrderDetails = o.OrderDetails.Where(od => od.BookId == bookId.Value).ToList();
                            return o;
                        }).Where(o => o.OrderDetails.Count != 0);
                    }

                    if (startDate.HasValue)
                    {
                        var formattedStartDate = startDate.Value.Date;
                        orders = orders.Where(o => o.Date >= formattedStartDate);
                    }

                    if (endDate.HasValue)
                    {
                        var formattedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                        orders = orders.Where(o => o.Date <= formattedEndDate);
                    }
                    report.Orders = orders;
                }
            }
            catch (Exception)
            {
            }
            return View(report);
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                IEnumerable<Book> books = await _bookRepository.GetAll();
                ServiceResponse ordResponse = await _orderRepository.GetAllOrder();
                IEnumerable<Order>? orders = ordResponse.Result as IEnumerable<Order>;

                Dashboard dashboard = new Dashboard() { 
                    BooksCount = books.Count(),
                    OrdersCount = orders.Count()
                };


            }
            catch (Exception) { 
            }
            return View();
        }
    }
}
