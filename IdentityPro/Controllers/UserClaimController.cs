using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityPro.Controllers
{
    public class UserClaimController : Controller
    {
        public readonly UserManager<User> _userManager;

        public UserClaimController(UserManager<User> user)
        {
            _userManager = user;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(User.Claims);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string ClaimType, string ClaimValue)
        {
            var user = _userManager.GetUserAsync(User).Result;
            Claim claim = new Claim(ClaimType , ClaimValue , ClaimValueTypes.String);
            var result = _userManager.AddClaimAsync(user, claim).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
