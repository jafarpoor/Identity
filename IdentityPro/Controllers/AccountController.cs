using IdentityPro.Models.Dto;
using IdentityPro.Models.Entites;
using IdentityPro.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Controllers
{
    public class AccountController : Controller
    {
        public readonly UserManager<User> _identityUser;
        public readonly SignInManager<User> _signInManager;
        private readonly EmailService emailService;


        public AccountController(UserManager<User> userManager , SignInManager<User> signInManager)
        {
            _identityUser = userManager;
            _signInManager = signInManager;
            emailService = new EmailService();
        }

        [Authorize]
        public IActionResult Index()
        {
            var user = _identityUser.FindByNameAsync(User.Identity.Name).Result;
            return View(new AccountentUserDto { 
                FullName = $"{user.FristName} {user.LastName}" ,
                Email = user.Email ,
                PhoneNumber = user.PhoneNumber ,
                UserName = user.UserName ,
                Id = user.Id ,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled
            });
        }

        [Authorize]
        public IActionResult TwoFactorEnabled()
        {
            var user = _identityUser.FindByNameAsync(User.Identity.Name).Result;
            var result = _identityUser.SetTwoFactorEnabledAsync(user, !user.TwoFactorEnabled).Result;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);
            User MyUSer = new User
            {
                FristName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email

            };
            var Result = _identityUser.CreateAsync(MyUSer, registerDto.PassWord).Result;
            if (Result.Succeeded)
            {
                var token = _identityUser.GenerateEmailConfirmationTokenAsync(MyUSer).Result;
                string callBackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    UserId = MyUSer.Id,
                    token = token
                }, protocol: Request.Scheme);
                string body = $"لطفا برای فعال حساب کاربری بر روی لینک زیر کلیک کنید!  <br/> <a href={callBackUrl}> Link </a>";
                emailService.Excute(MyUSer.Email, body, "ارسال ایمیل فعال سازی");
                return RedirectToAction("DisplayEmail");
            }
               

            string Message = "";
            foreach (var item in Result.Errors.ToList())
            {
                Message += item.Description + Environment.NewLine;
            }

            TempData["Message"] = Message;
            return View(registerDto);
        }

        public IActionResult ConfirmEmail(string UserId , string Token)
        {
            if (UserId == null || Token == null)
                return BadRequest();
            var user = _identityUser.FindByIdAsync(UserId).Result;
            if (user == null)
            {
                return View("Error");

            }
            var result = _identityUser.ConfirmEmailAsync(user, Token).Result;
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                _identityUser.UpdateAsync(user);
            }
            else
            {
                //
            }


            return RedirectToAction("Login");
        }

        public IActionResult DisplayEmail()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl="/")
        {
            return View(new LoginDto
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            //برای اینکه اگر کاربر لاگین بود خواست دوباره لاگین کنه خطا نده


            _signInManager.SignOutAsync();
            var User = _identityUser.FindByNameAsync(loginDto.UserName).Result;
            if(User == null)
                return View(loginDto);


            var Result = _signInManager.PasswordSignInAsync(User, loginDto.Password, loginDto.IsPersistent
                 , true).Result;

            if (Result.Succeeded)
                return Redirect(loginDto.ReturnUrl);

            if (Result.RequiresTwoFactor)
            {
                return RedirectToAction("TwoFactorLogin" , new { loginDto.UserName , loginDto.IsPersistent});
            }

           if (Result.IsLockedOut) { //
                                   }
                    
            return View();
        }

        public IActionResult TwoFactorLogin(string UserName, bool IsPersistent)
        {
            var user = _identityUser.FindByNameAsync(UserName).Result;
            if (user == null)
                return BadRequest();
            var provider = _identityUser.GetValidTwoFactorProvidersAsync(user).Result;
            TwoFactorLoginDto twoFactorLoginDto = new TwoFactorLoginDto();
            if (provider.Contains("Phone"))
            {
                string smsCode = _identityUser.GenerateTwoFactorTokenAsync(user ,"Phone").Result;
                SmsService serivice = new SmsService();
                serivice.Send(user.PhoneNumber, smsCode);
                twoFactorLoginDto.IsPersistent = IsPersistent;
                twoFactorLoginDto.Provider = "Phone";

            }
            else if (provider.Contains("Email"))
            {
                string emailCode = _identityUser.GenerateTwoFactorTokenAsync(user, "Email").Result;
                EmailService emailService = new EmailService();
                emailService.Excute(user.Email, $"Two Factor Code:{emailCode}", "TwoFactorLogin");
                twoFactorLoginDto.Provider = "Email";
                twoFactorLoginDto.IsPersistent = IsPersistent;
            }
            return View(twoFactorLoginDto);
        }

        [HttpPost]
        public IActionResult TwoFactorLogin(TwoFactorLoginDto twoFactorLoginDto)
        {
            if (!ModelState.IsValid)
                return View(twoFactorLoginDto);
            var user = _signInManager.GetTwoFactorAuthenticationUserAsync().Result;
            if (user == null)
                return BadRequest();
            var result = _signInManager.TwoFactorSignInAsync(twoFactorLoginDto.Provider, twoFactorLoginDto.Code, twoFactorLoginDto.IsPersistent, false).Result;
            if (result.Succeeded)
                return RedirectToAction("Index");
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "حساب کاربری شما قفل شده است");
                return View();
            }
            else
            {
                ModelState.AddModelError("", "کد وارد شده صحیح نمی باشد");
                return View();
            }
        }


        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = _identityUser.FindByEmailAsync(forgotPasswordDto.Email).Result;
            if(user ==null || _identityUser.IsEmailConfirmedAsync(user).Result)
            {
                ViewBag.meesage = "ممکن است ایمیل وارد شده معتبر نباشد! و یا اینکه ایمیل خود را تایید نکرده باشید";
                return View("Error");
            }

            var token = _identityUser.GeneratePasswordResetTokenAsync(user).Result;
            var callbakUrl = Url.Action("ResetPassword", "Account", new
            {
                token = token,
                uerId = user.Id
            }, protocol: Request.Scheme);

            string body = $"برای تنظیم مجدد کلمه عبور بر روی لینک زیر کلیک کنید <br/> <a href={callbakUrl}> link reset Password </a>";
            emailService.Excute(user.Email, body, "فراموشی رمز عبور");
            ViewBag.meesage = "لینک عوض کردن ایمیل برای شما ارسال شد";
            return View();

        }

        public IActionResult ResetPassword(string userId, string tokrn)
        {
            return View(new RestPasswordDto { 
                UserId = userId ,
                Token = tokrn ,
            });
        }

        [HttpPost]
        public IActionResult ResetPassword(RestPasswordDto restPasswordDto )
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = _identityUser.FindByIdAsync(restPasswordDto.UserId).Result;
            if (user == null)
                return BadRequest();
            if (restPasswordDto.Password != restPasswordDto.ConfirmPassword)
                return BadRequest();

            var result = _identityUser.ResetPasswordAsync(user, restPasswordDto.Token, restPasswordDto.Password).Result;
            if (result.Succeeded)
                return RedirectToAction("ChangePassword");
            else
                return BadRequest();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult SetPhoneNumber()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetPhoneNumber(SetPhoneNumberDto setPhoneNumberDto)
        {
            var user = _identityUser.FindByNameAsync(User.Identity.Name).Result;
            var SetResult = _identityUser.SetPhoneNumberAsync(user, setPhoneNumberDto.PhoneNumber).Result;
            var token = _identityUser.GenerateChangePhoneNumberTokenAsync(user, setPhoneNumberDto.PhoneNumber).Result;
            SmsService smsService = new SmsService();
            smsService.Send(user.PhoneNumber, token);
            TempData["PhoneNumber"] = setPhoneNumberDto.PhoneNumber;
            return RedirectToAction(nameof(VerifyPhoneNumber));
        }

        public IActionResult VerifyPhoneNumber()
        {
            return View(new VerifyPhoneNumberDto
            {
                PhoneNumber = TempData["PhoneNumber"].ToString()
            }) ;
        }


        [Authorize]
        [HttpPost]
        public IActionResult VerifyPhoneNumber(VerifyPhoneNumberDto verifyPhoneNumberDto)
        {
            var user = _identityUser.FindByNameAsync(User.Identity.Name).Result;
            bool result = _identityUser.VerifyChangePhoneNumberTokenAsync(user, verifyPhoneNumberDto.Code, verifyPhoneNumberDto.PhoneNumber).Result;
            if (!result)
            {
                ViewData["Message"] = $"کد وارد شده برای این شماره تلفن اشتباه می باشد {verifyPhoneNumberDto.PhoneNumber} ";
                return View(verifyPhoneNumberDto);
            }
            else
            {
                user.PhoneNumberConfirmed = true;
                _identityUser.UpdateAsync(user);
            }
            return RedirectToAction(nameof(VerifySuccess));
        }

        public IActionResult VerifySuccess()
        {
            return View();
        }
    }
}
