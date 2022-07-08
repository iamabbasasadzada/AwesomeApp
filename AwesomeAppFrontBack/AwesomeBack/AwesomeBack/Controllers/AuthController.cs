using AwesomeBack.Models;
using AwesomeBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AwesomeBack.Controllers
{

    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return BadRequest();

            AppUser user = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                Email = registerVM.Email,
                UserName = registerVM.Username
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                }
                return View();
            }
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signIn, string ReturnUrl)
        {
            AppUser user;
            if (signIn.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(signIn.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(signIn.UserNameOrEmail);
            }
            if (user == null)
            {
                ModelState.AddModelError("", "Login or Password is incorrect");
                return View();

            }
            var result = await _signInManager.PasswordSignInAsync(user, signIn.Password, true, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your accont blocked for 5 minute");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login or Password is incorrect");
                return View();

            }
            if (ReturnUrl != null) return LocalRedirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
