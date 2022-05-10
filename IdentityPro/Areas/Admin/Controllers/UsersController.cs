using IdentityPro.Areas.Admin.Models.Dto;
using IdentityPro.Models.Dto;
using IdentityPro.Models.Entites;
using IdentityPro.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        public readonly UserManager<User> _identityUser;
        private readonly RoleManager<Role> _roleManager;
   
        public UsersController(UserManager<User> UserMange , RoleManager<Role> roleManager)
        {
            _identityUser = UserMange;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(_identityUser.Users.ToList().Select(p => new UserDto { 
                PhoneNumber = p.PhoneNumber,
                FirstName = p.FristName ,
                Id= p.Id ,
                LastName = p.LastName ,
                UserName = p.Email,
                Email = p.Email
            }));
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
                UserName = registerDto.UserName

            };
            var Result = _identityUser.CreateAsync(MyUSer, registerDto.PassWord).Result;
            if (Result.Succeeded)
            {
                return RedirectToAction("Index", "Users");

            }
                

            string Message = "";
            foreach (var item in Result.Errors.ToList())
            {
                Message += item.Description + Environment.NewLine;
            }

            TempData["Message"] = Message;
            return View(registerDto);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var Result = _identityUser.FindByIdAsync(id).Result;
            UserEditDto user = new UserEditDto()
            {
                Id = Result.Id,
                FirstName = Result.FristName,
                LastName = Result.LastName,
                PhoneNumber = Result.PhoneNumber,
                UserName = Result.UserName
            };

            return View(user);

        }

        [HttpPost]
        public IActionResult Edit(UserEditDto userDto)
        {
            var user = _identityUser.FindByIdAsync(userDto.Id).Result;
            user.FristName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.UserName = userDto.UserName;
            user.PhoneNumber = userDto.PhoneNumber;

            var Result = _identityUser.UpdateAsync(user).Result;
            if (Result.Succeeded)
                return RedirectToAction("Index", "Users", new { area = "Admin" });

            else
                return NotFound();
        }

        [HttpGet]
        public IActionResult Delet(string Id)
        {
            var user = _identityUser.FindByIdAsync(Id).Result;
            DeletUserDto deletUserDto = new DeletUserDto()
            {
                Id = user.Id,
                FirstName = user.FristName,
                LastName = user.LastName,
                UserName = user.LastName
            };
            return View(deletUserDto);
        }

        [HttpPost]
        public IActionResult Delet(DeletUserDto deletUserDto)
        {
            var user = _identityUser.FindByIdAsync(deletUserDto.Id).Result;
            var Result = _identityUser.DeleteAsync(user).Result;
            if (Result.Succeeded)
                return RedirectToAction("Index", "Users", new { area = "Admin" });
            else
                return NotFound();

        }


        public IActionResult AddUserRole(string id)
        {
            var user = _identityUser.FindByIdAsync(id).Result;
            var roles = new List<SelectListItem>(_roleManager.Roles.Select(p => new SelectListItem {
                Text = p.Name,
                Value = p.Name
            }).ToList());

            return View(new AddUserRoleDto
            {
                IdUser = id ,
                Email = user.UserName,
                FullName = $"{user.FristName} {user.LastName}",
                Roles = roles
            });
        }

        [HttpPost]
        public IActionResult AddUserRole(AddUserRoleDto addUserRoleDto)
        {
            var user = _identityUser.FindByIdAsync(addUserRoleDto.IdUser).Result;
            var Result = _identityUser.AddToRoleAsync(user, addUserRoleDto.Role).Result;
            if (Result.Succeeded)
                return RedirectToAction("UserRoles", "Users", new { Id = user.Id, area = "Admin" });

            else
                return NotFound();
        }

        public IActionResult UserRoles(string Id)
        {
            var user = _identityUser.FindByIdAsync(Id).Result;
            var roles = _identityUser.GetRolesAsync(user).Result;
            ViewBag.UserInfo = $"{user.FristName}  {user.LastName}";
            return View(roles);
        }
    }
}
