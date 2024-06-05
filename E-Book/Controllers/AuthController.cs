using E_Book.Areas.Identity.Pages.Account;
using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Win32;

namespace E_Book.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet("SignIn")]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpGet("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(LoginDTO login)
        {
            try
            {
                ServiceResponse response = await _authRepository.Login(login);
                if (response.IsSuccess)
                {
                    TempData["success"] = "Successfully Logged in";
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = response.Result;
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return View(login);
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(RegisterDTO register)
        {
            try
            {
                ServiceResponse response = await _authRepository.Register(register);
                if (response.IsSuccess)
                {
                    TempData["success"] = "Successfully Registered";
                    return RedirectToAction("SignIn");
                }
                TempData["error"] = response.Result;
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return View(register);
        }

        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            try
            {
                ServiceResponse response = await _authRepository.Logout();

                if (response.IsSuccess)
                    TempData["success"] = "Successfully signed out.";
                else
                    TempData["error"] = response.Result;

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Customers")]
        public async Task<IActionResult> Customers()
        {
            IEnumerable<UserWithRolesDTO> users = null;
            try
            {
                ServiceResponse response = await _authRepository.GetCustomer();
                if (response.IsSuccess)
                {
                    users = (IEnumerable<UserWithRolesDTO>)response.Result;
                    return View(users);
                }

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return View(users);
        }

    }
}
