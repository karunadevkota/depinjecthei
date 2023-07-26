using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // Redirect to admin dashboard after successful login
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Username or Password");
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Register(RegisterViewModel registermodel)
        {
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registermodel.Email,
                    Email = registermodel.Email

                };
                var result = await _userManager.CreateAsync(user,registermodel.Password);
                if(result.Succeeded)
                {
                    // AFter successgfull registeration redirect user to login page 
                    return RedirectToAction("Login");
                }
                //email already exist xa ki nai check garna ko lagi 
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateEmail")
                    {
                        //email alreday exist
                        ModelState.AddModelError(nameof(registermodel.Email), "The email address is already registered.");

                    }

                    else
                    {

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(registermodel);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult>ForgotPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword","Account",new {userId= user.Id,token}, protocol :HttpContext.Request.Scheme);
                    // send the password reset link to the user's email using the callbackUrl

                }
                //redirect to login 
                return RedirectToAction("Login");
            }
            // If email not found or model is invalidd display the  ForgotPassword view with errors
            return View (model);
        }
       // [Authorize] // Add this attribute to the ChangePassword action
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        // password changed successfully 
                        return RedirectToAction("Index", "Home");
                    }
                    // yooo chai current paassword correct xa ki  nai check garna ko lagi 
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "PasswordMismatch")
                        {
                            // Add a custom error message for the CurrentPassword field
                            ModelState.AddModelError(nameof(model.CurrentPassword), "The current password is incorrect.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                
                else
                {
                    // User not found
                    ModelState.AddModelError(string.Empty, "User not found.");
                }

            }

            // if password changed fails or model is invalid 
            return View(model);

        }


    }
    }
