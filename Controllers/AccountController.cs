using AuthApp.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApp.Models;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;

            this.userManager = userManager;

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password!, isPersistent: false, lockoutOnFailure: false);

                    if (user.IsBlocked)
                    {
                        ModelState.AddModelError("", "This account is blocked.");
                        return View(model);
                    }

                    if (result.Succeeded)
                    {
                        user.LastLogin = DateTime.Now;
                        await userManager.UpdateAsync(user);
                        return RedirectToAction("UserManagement", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }


        public async Task<IActionResult> UserManagement()
        {
            var users = await userManager.Users.Select(u => new UserManagementVM
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                LastLogin = u.LastLogin,
                IsBlocked = u.IsBlocked
            }).ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(List<string> selectedUsers)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = true;
                    await userManager.UpdateAsync(user);
                }
            }
            if (selectedUsers.Count == userManager.Users.Count())
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUsers(List<string> selectedUsers)
        {
            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = false;
                    await userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction("UserManagement");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(List<string> selectedUsers)
        {
            var currentUser = await userManager.GetUserAsync(User);
            bool isCurrentUserDeleted = false;

            foreach (var userId in selectedUsers)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await userManager.DeleteAsync(user);

                    if (currentUser != null && currentUser.Id == userId && result.Succeeded)
                    {
                        isCurrentUserDeleted = true;
                    }
                }
            }

            if (isCurrentUserDeleted)
            {
                await signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("UserManagement");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    UserName = model.Name,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.Now;
                    await userManager.UpdateAsync(user);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("UserManagement", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}