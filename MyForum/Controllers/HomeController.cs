using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyForum.Models;
using MyForum.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ForumDBContext context;

        public HomeController(ILogger<HomeController> logger, ForumDBContext _context)
        {
            _logger = logger;
            context = _context;
        }
        public async Task<IActionResult> Index()
        {
            User user = await context.Users
                      .Include(u => u.Avatar)
                      .Include(u => u.Role)
                      .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            ChangePasswordViewModel changePassModel = new ChangePasswordViewModel();
            var list = Enumerable.Reverse(await context.Users
                .Include(u => u.Avatar)
                .Include(u => u.Role).ToListAsync()).Take(5).ToList();
            //var list2 = Enumerable.Reverse(await context.Threads
            //    .Include(u => u.Autor)
            //    .ToListAsync()).Take(5).ToList();
            var list2 = context.Threads
                .Include(u => u.Autor)
                .OrderByDescending(d => d.LastUpdate).Take(5).ToList();
            return View(Tuple.Create(user, changePassModel,list, list2));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
