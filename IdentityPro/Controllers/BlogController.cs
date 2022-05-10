using IdentityPro.Data;
using IdentityPro.Models.Dto;
using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IdentityPro.Controllers
{
    public class BlogController : Controller
    {
            private DataBaseContext _context;
            private UserManager<User> _userManager;
            private IAuthorizationService _authorization;
            public BlogController(DataBaseContext context, UserManager<User> userManager, IAuthorizationService authorization)
            {
                _context = context;
                _userManager = userManager;
                _authorization = authorization;
            }

            public IActionResult Index()
            {
                var Blags = _context.Blogs
                            .Include(p => p.User)
                            .Select(p => new BlogDto
                            {
                                Id = p.Id,
                                Body = p.Body,
                                Title = p.Title,
                                UserName = p.User.UserName
                            });

                return View(Blags);

            }

            [HttpGet]
            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Create(BlogDto blogDto)
            {
                var user = _userManager.GetUserAsync(User).Result;
                Blog MyBlog = new Blog()
                {
                    Body = blogDto.Body,
                    Title = blogDto.Title,
                    User = user,
                };
                _context.Add(MyBlog);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            public IActionResult Edit(long Id)
            {
                var blog = _context.Blogs
                            .Include(p => p.User)
                            .Where(p => p.Id == Id)
                            .Select(p => new BlogDto
                            {
                                Body = p.Body,
                                Id = p.Id,
                                Title = p.Title,
                                UserId = p.UserId,
                                UserName = p.User.UserName
                            }).FirstOrDefault();

                var result = _authorization.AuthorizeAsync(User, blog, "IsBlogForUser").Result;

                if (result.Succeeded)
                {
                    return View(blog);
                }
                else
                {
                    return NotFound();
                }
            }

            [HttpPost]
            public IActionResult Edit(BlogDto blogDto)
            {
                return View();
            }
        }
    }