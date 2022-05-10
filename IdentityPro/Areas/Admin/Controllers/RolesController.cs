using IdentityPro.Areas.Admin.Models.Dto;
using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        public readonly UserManager<User> _identityUser;

        public RolesController(RoleManager<Role> roleManager , UserManager<User>  userManager )
        {
            _roleManager = roleManager;
            _identityUser = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList().Select(p => new RoleListDto
            {
                Id = p.Id ,
                Name = p.Name ,
                Description = p.Description
            }).ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AddNewRole roleListDto)
        {
            Role role = new Role()
            {
                Name = roleListDto.Name,
                Description = roleListDto.Description
            };
            var Result = _roleManager.CreateAsync(role).Result;
            if (Result.Succeeded)
                return RedirectToAction("Index", "Roles", new { area = "Admin" });
            else
                return NotFound();

        }


        //    _roleManager.UpdateAsync();
        //    _roleManager.DeleteAsync();

        public IActionResult UserInRole(string Name)
        {
            var user = _identityUser.GetUsersInRoleAsync(Name).Result;
            return View(user.Select(p => new UserDto { 
                Id =p.Id,
                FirstName = p.FristName ,
                LastName = p.LastName 
            }).ToList());
   
        }
    }
}
