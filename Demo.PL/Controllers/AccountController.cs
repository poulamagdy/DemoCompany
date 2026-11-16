using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailSettings _mailSettings;
        private readonly ISmsService _smsService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMailSettings mailSettings, ISmsService smsService)
        {
			_userManager = userManager;
			_signInManager = signInManager;
            _mailSettings = mailSettings;
            _smsService = smsService;
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
		public async Task<IActionResult> SignUp(SignUpViewModel model)
		{
            if (ModelState.IsValid) 
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email.Split("@")[0],
                    Email = model.Email,
                    IsActive = model.IsActive,
                };

                var result = await _userManager.CreateAsync(user,model.Password);

                if (result.Succeeded)
                    return RedirectToAction("Login");
                else
                {
                    foreach(var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
			return View(model);
		}
        public ActionResult GoogleLogin()
        {
            var pop = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(pop, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<ActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(c => new
            {
                c.Issuer,
                c.OriginalIssuer,
                c.Type,
                c.Value
            });
			return RedirectToAction("Index", "Home");
		}
		public IActionResult Login()
		{
			return View();
		}
        [HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
            if (ModelState.IsValid) 
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    ModelState.AddModelError(string.Empty, "Email Not Exist");

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
                else
					ModelState.AddModelError(string.Empty, "Email or Password Does Not Exist");
			}
			return View(model);
		}
		public async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
		}

		public IActionResult ForgetPassword()
		{
            return View();
		}

        [HttpPost]
		public async Task<IActionResult> SentEmail(ForgetPasswordViewModel model)
		{
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    ModelState.AddModelError(string.Empty, "Email Not EXIST");

                if (user != null)
                {
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var SendpasswordLink = Url.Action("ResetPassword", "Account", new {email = user.Email, token = Token},Request.Scheme);

                    var email = new Email
                    {
                        To = user.Email,
                        Subject = "Reset Password",
                        Body = SendpasswordLink

					};

                    _mailSettings.SendEmail(email);
                    return RedirectToAction(nameof(CheckInBox));
                }
                else
					ModelState.AddModelError(string.Empty, "Email Not EXIST");
			}
			return View("ForgetPassword", model);
		}

        public IActionResult CheckInBox()
        {
            return View();
        }

		public IActionResult ResetPassword(string email, string token)
		{
            TempData["email"]= email;
            TempData["token"]= token;
			return View();
		}
        [HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{

			if (ModelState.IsValid) 
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                    ModelState.AddModelError(string.Empty, "Email Does Not Exist");

                if (user != null) 
                {
                    var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                    if (result.Succeeded)
                        return RedirectToAction(nameof(Login));
                    else
                    {
                        foreach (var item in result.Errors)
                            ModelState.AddModelError(string.Empty, item.Description);
                    }
                }
            }
            return View(model);
		}

        public ActionResult CheckSms()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> SentSms(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                    ModelState.AddModelError(string.Empty, "Email Not EXIST");

                if (user != null)
                {
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var SendpasswordLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token = Token }, Request.Scheme);

                    var sms = new Sms
                    {
                        PhoneNumber = user.PhoneNumber,
                        Body = SendpasswordLink

                    };

                    _smsService.SendSms(sms);
                    return RedirectToAction(nameof(CheckSms));
                }
                else
                    ModelState.AddModelError(string.Empty, "Email Not EXIST");
            }
            return View("ForgetPassword", model);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
