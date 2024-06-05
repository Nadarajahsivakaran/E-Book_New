using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Book.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepository;

        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

     
        public async Task<IActionResult> Index(string search)
        {
            IEnumerable<Book> books = [];
            try
            {
                if (string.IsNullOrEmpty(search)) 
                       books = await _bookRepository.GetAll();
                else
                    books = await _bookRepository.GetAll("",search);
                ViewBag.Search = search;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
            }
            return View(books);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
