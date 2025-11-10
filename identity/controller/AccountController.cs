using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using movie_hospital_1.dataModel;
using movie_hospital_1.Migrations;
using movie_hospital_1.Reposotories.IReposotories;
using System.Threading.Tasks;

namespace movie_hospital_1.identity.controller
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        private readonly IEmailSender _emailSender;

        public IRepossitory<ApplicationUserOTP> _ApplicationUserOTPRepossitory;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInManager, IEmailSender emailSender, IRepossitory<ApplicationUserOTP> applicationUserOTPRepossitory
            )
        {
            _userManager = userManager;
            _SignInManager = SignInManager;
            _emailSender = emailSender;
            _ApplicationUserOTPRepossitory = applicationUserOTPRepossitory;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);
            var user = new ApplicationUser()
            {
                firstName = registerVM.firstName,
                lastName = registerVM.lastName,
                Email = registerVM.Email,
                UserName = registerVM.userName,
            };
            var result = await _userManager.CreateAsync(user, registerVM.password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }
                return View(registerVM);
            }

            // send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(ConfirmEmail), "Account", new { token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(
             registerVM.Email,
              "Movie Cinema - Confirm your email",
             $"<h1>Please confirm your email by clicking <a href='{link}'>here</a></h1>");

            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                TempData["Error-notification"] = " invalid user ";


            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                TempData["Error-notification"] = " invalid tokwn ";
            else
                TempData["success-notification"] = " confirm email successfully ";

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);
            var user = await _userManager.FindByNameAsync(loginVM.UserOREmail) ??
                await _userManager.FindByEmailAsync(loginVM.UserOREmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Email or password");
                return View(loginVM);
            }
            var result = await _SignInManager.PasswordSignInAsync(user, loginVM.password, loginVM.RememberMe, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Your account is locked please try again After 5 min");
                else if (!user.EmailConfirmed)

                    ModelState.AddModelError(string.Empty, "Please confirm your email first");
                else
                    ModelState.AddModelError(string.Empty, "Invalid UserName / Email or password");
                return View(loginVM);



            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
                return View(resendEmailConfirmationVM);
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.usernameOrEmail) ??
                await _userManager.FindByEmailAsync(resendEmailConfirmationVM.usernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Email");
                return View(resendEmailConfirmationVM);
            }
            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email already confirmed");
                return View(resendEmailConfirmationVM);
            }
            // send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var link = Url.Action(nameof(ConfirmEmail), "Account", new { token, userId = user.Id }, Request.Scheme);

            await _emailSender.SendEmailAsync(
             user.Email,
              "Movie Cinema - Confirm your email",
             $"<h1>Please confirm your email by clicking <a href='{link}'>here</a></h1>");
            return RedirectToAction("Login");

        }
        public ActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByNameAsync(forgetPasswordVM.usernameOrEmail) ??
                await _userManager.FindByEmailAsync(forgetPasswordVM.usernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid UserName / Email");
                return View(forgetPasswordVM);
            }

            var userOTPs = await _ApplicationUserOTPRepossitory.GetAsync(e => e.ApplicationUserId == user.Id);

            var totalOTPs = userOTPs.Count(e => (DateTime.UtcNow - e.createAt).TotalHours < 24);
            if (totalOTPs > 3)
            {
                ModelState.AddModelError(string.Empty, "You have exceeded the maximum number of OTP requests. Please try again later.");
                return View(forgetPasswordVM);
            }
            var OTP = new Random().Next(1000, 9999);
            await _emailSender.SendEmailAsync(
             user.Email,
              "Movie Cinema - Confirm your password",
             $"<h1>use this OTP:  {OTP} to reseat your account. don,t share it  </h1>");
            await _ApplicationUserOTPRepossitory.Add(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                createAt = DateTime.UtcNow,
                OTP = OTP.ToString(),
                validTo = DateTime.UtcNow.AddDays(1),

            });
            _ApplicationUserOTPRepossitory.Commit();
            return RedirectToAction("ValidateOTP", new { userId = user.Id });

        }
        public ActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM
            {
                ApplicationUserId = userId
            });

        }
        [HttpPost]
        public async Task<ActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var result = await _ApplicationUserOTPRepossitory.GetOne(e =>
                e.ApplicationUserId == validateOTPVM.ApplicationUserId &&
                e.OTP == validateOTPVM.OTP &&
                e.isValid
            );

            if (result is  null)
            {
                TempData["ErrorMessage"] = "Invalid OTP";
                return RedirectToAction(nameof(ValidateOTP), new { userId = validateOTPVM.ApplicationUserId });
            }

            return RedirectToAction("NewPassword", new { userId = validateOTPVM.ApplicationUserId });
        }



        public ActionResult NewPassword(string userId)
        {
            return View(new newPasswordVM
            {
                ApplicationUserId = userId
            });

        }
        [HttpPost]
        public async Task<ActionResult> NewPassword(newPasswordVM newPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(newPasswordVM.ApplicationUserId);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid User");
                return View(newPasswordVM);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }
                return View(newPasswordVM);

            }

            return RedirectToAction("Login");
        }

    }
}













