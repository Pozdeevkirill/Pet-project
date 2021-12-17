using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForum.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.Controllers
{
    public class ThreadController: Controller
    {
        private ForumDBContext context;

        public ThreadController(ForumDBContext context)
        {
            this.context = context;
        }
        #region Create Thread
        [Authorize]
        public IActionResult CreateThread()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(string title,string content)
        {
            Threads th = await context.Threads.FirstOrDefaultAsync(u => u.Title == title);
            if(th == null)
            {
                if(content.Length >= 50)
                {
                    User user = await context.Users
                     .Include(u => u.Avatar)
                     .Include(u => u.Role)
                     .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
                    Threads thread = new Threads { Autor = user, Title = title, Content = content, DataPublish = DateTime.Now };
                    context.Threads.Add(thread);
                    await context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", $"Содержимое тема не должно быть таким кортоким!");
                }
            }
            else
            {
                ModelState.AddModelError("", $"Такое название темы уже существует!");
            }
            
            return View();
        }
        #endregion
        public async Task<IActionResult> Thread(int id)
        {
            User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            Threads th = await context.Threads
                    .Include(u => u.Autor)
                    .Include(u => u.Autor.Avatar)
                    .Include(r => r.Autor.Role)
                    .FirstOrDefaultAsync(i => i.Id == id);
            var com = Enumerable.Reverse(await context.Comments
                .Include(u => u.Users)
                .Include(th => th.Thread).ToListAsync()).ToList();
            return View(Tuple.Create(user,th,com));
        }
    }
}
