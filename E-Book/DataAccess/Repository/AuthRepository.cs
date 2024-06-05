using E_Book.DataAccess.IRepository;
using E_Book.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Plugins;

namespace E_Book.DataAccess.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ServiceResponse _serviceResponse;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthRepository(UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
                                ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _serviceResponse = new ServiceResponse();
            _applicationDbContext = applicationDbContext;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponse> Register(RegisterDTO register)
        {
            try
            {
                ApplicationUser? isEmailExit = await _userManager.FindByEmailAsync(register.Email);

                if (isEmailExit != null)
                {
                    _serviceResponse.IsSuccess = false;
                    _serviceResponse.Result = "The Email already Exist";
                    return _serviceResponse;
                }

                ApplicationUser user = new()
                {
                    Email = register.Email,
                    FirstName = register.FirstName,
                    UserName = register.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    ApplicationUser? newUser = await _applicationDbContext.ApplicationUser
                     .FirstOrDefaultAsync(u => u.UserName == register.Email);

                    string roleName = register.Role.ToString().ToUpper();
                    bool roleExist = await _roleManager.RoleExistsAsync(roleName);

                    if (!roleExist)
                    {
                        IdentityRole role = new(roleName);
                        await _roleManager.CreateAsync(role);
                    }

                    if (newUser != null)
                        await _userManager.AddToRoleAsync(newUser, roleName);

                    _serviceResponse.IsSuccess = true;
                    return _serviceResponse;
                }
                _serviceResponse.Result = result.Errors;
            }
            catch (Exception ex)
            {
                _serviceResponse.Result = ex.Message.ToString();
            }
            _serviceResponse.IsSuccess = false;
            return _serviceResponse;
        }


        public async Task<ServiceResponse> Login(LoginDTO login)
        {
            try
            {
                ApplicationUser? user = await _userManager.FindByNameAsync(login.UserName);

                if (user != null)
                {
                    bool isPasswordOk = await _userManager.CheckPasswordAsync(user, login.Password);

                    if (isPasswordOk)
                    {
                        IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
                        user.RoleName = string.Join(", ", roles);

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        _serviceResponse.IsSuccess = true;
                        _serviceResponse.Result = user;
                        return _serviceResponse;
                    }
                }
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = "Invalid UserName or Password";
                return _serviceResponse;
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex.ToString();
                return _serviceResponse;
            }
        }

        public async Task<ServiceResponse> Logout()
        {
            try
            {
                _serviceResponse.IsSuccess = true;
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex.ToString();
            }
            return _serviceResponse;
        }

        public async Task<ServiceResponse> GetCustomer()
        {
            ServiceResponse _serviceResponse = new ServiceResponse();

            try
            {
                // Fetch all users from the database
                IEnumerable<ApplicationUser> users = await _applicationDbContext.ApplicationUser.ToListAsync();

                // Create a list to hold users with their roles
                List<UserWithRolesDTO> usersWithRoles = new List<UserWithRolesDTO>();

                foreach (var user in users)
                {
                    // Fetch the roles for the current user
                    var roles = await _userManager.GetRolesAsync(user);

                    // Check if the user has the "User" role and not the "Admin" role
                    if (roles.Contains("USER") && !roles.Contains("ADMIN"))
                    {
                        // Create a new UserWithRolesDTO object
                        UserWithRolesDTO userWithRoles = new UserWithRolesDTO
                        {
                            Id = user.Id,
                            Email = user.Email,
                            UserName = user.FirstName,
                            Roles = roles[0]
                        };

                        // Add the UserWithRolesDTO object to the list
                        usersWithRoles.Add(userWithRoles);
                    }
                }

                // Set the service response
                _serviceResponse.IsSuccess = true;
                _serviceResponse.Result = usersWithRoles;
            }
            catch (Exception ex)
            {
                // Handle any errors
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex.ToString();
            }

            return _serviceResponse;
        }

        public async Task<ServiceResponse> GetCustomerAdmin()
        {
            ServiceResponse _serviceResponse = new ServiceResponse();

            try
            {
                IEnumerable<ApplicationUser> users = await _applicationDbContext.ApplicationUser.ToListAsync();
                List<UserWithRolesDTO> usersWithRoles = new List<UserWithRolesDTO>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                        UserWithRolesDTO userWithRoles = new UserWithRolesDTO
                        {
                            Id = user.Id,
                            Email = user.Email,
                            UserName = user.FirstName,
                            Roles = roles[0]
                        };
                        usersWithRoles.Add(userWithRoles);
                }
                _serviceResponse.IsSuccess = true;
                _serviceResponse.Result = usersWithRoles;
            }
            catch (Exception ex)
            {
                _serviceResponse.IsSuccess = false;
                _serviceResponse.Result = ex.ToString();
            }
            return _serviceResponse;
        }
    }
}
