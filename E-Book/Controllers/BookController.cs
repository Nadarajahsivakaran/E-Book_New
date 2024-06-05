using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Reflection.Metadata.BlobBuilder;

namespace E_Book.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BookController(IBookRepository bookRepository, ILogger<BookController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Book> books = [];
            try
            {
                books = await _bookRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
            }
            return View(books);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateBook createBook)
        {
            try
            {
                Book book = new()
                {
                    Title = createBook.Title,
                    Author = createBook.Author,
                    Description = createBook.Description,
                    Price = createBook.Price,
                    PublishedDate = createBook.PublishedDate,
                    NoOfCopies = createBook.NoOfCopies
                };

                if (createBook.ImagePath != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + createBook.ImagePath.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await createBook.ImagePath.CopyToAsync(fileStream);
                    }
                    book.Image = "/images/" + uniqueFileName;
                }

                await _bookRepository.Create(book);
                TempData["success"] = "Successfully Created";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "An error occurred while adding books.");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {

            try
            {
                Book book = await _bookRepository.GetData(b => b.Id == id);

                CreateBook createBook = new()
                {
                    Id = id,
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    Price = book.Price,
                    PublishedDate = book.PublishedDate,
                    NoOfCopies = book.NoOfCopies
                };
                return View(createBook);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "An error occurred while fetching books.");
                return View();
            }

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(CreateBook createBook)
        {
            try
            {
                Book book = new()
                {
                    Id = createBook.Id,
                    Title = createBook.Title,
                    Author = createBook.Author,
                    Description = createBook.Description,
                    Price = createBook.Price,
                    PublishedDate = createBook.PublishedDate,
                    NoOfCopies = createBook.NoOfCopies
                };

                if (createBook.ImagePath != null)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + createBook.ImagePath.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await createBook.ImagePath.CopyToAsync(fileStream);
                    }
                    book.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    Book getbook = await _bookRepository.GetData(b => b.Id == createBook.Id);
                    book.Image = getbook.Image;
                }
                await _bookRepository.Update(book);
                TempData["success"] = "Successfully Updated";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "An error occurred while update books.");
            }
            return RedirectToAction("Index");
        }

        [HttpPost("AddFeedBack")]
        public async Task<IActionResult> AddFeedBack(string feedBack, int bookId)
        {
            try
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    await _bookRepository.AddFeedBack(userId, feedBack, bookId);
                    TempData["success"] = "Successfully Added";
                    return RedirectToAction("BookView", "Order", new { id = bookId });
                }
                else
                {
                    return RedirectToAction("SignIn", "Auth");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                _logger.LogError(ex, "An error occurred while update books.");
            }
            return RedirectToAction("Index");
        }
    }
}
