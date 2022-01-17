using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForum.Data.Models;
using MyForum.ViewModels;
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
        public async Task<IActionResult> CreateThread(string title,string content, string commentType)
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
                    Threads thread = new Threads { Autor = user, Title = title, Content = content, DataPublish = DateTime.Now, LastUpdate = DateTime.Now};
                    if (commentType == "Все пользователи")
                        thread.CommentType = 1;
                    else if(commentType == "Только модераторы и администраторы")
                        thread.CommentType = 2;
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
               .Include(u => u.Users.Avatar)
               .Include(u => u.Users.Role)
               .Include(th => th.Thread).ToListAsync()).ToList();
            com.RemoveAll(u => u.Users != th.Autor);
            return View(Tuple.Create(user,th,com));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteThread(int id)
        {
           Threads th = await context.Threads
                   .Include(u => u.Autor)
                   .Include(u => u.Autor.Avatar)
                   .Include(r => r.Autor.Role)
                   .FirstOrDefaultAsync(i => i.Id == id);
            var com = Enumerable.Reverse(await context.Comments
                   .Include(u => u.Users)
                   .Include(u => u.Users.Avatar)
                   .Include(u => u.Users.Role)
                   .Include(th => th.Thread).ToListAsync()).ToList();
            foreach (var c in com)
            {
                if(c.Thread == th)
                {
                    context.Comments.Remove(c);
                    await context.SaveChangesAsync();
                }
            }
            context.Threads.Remove(th);
            await context.SaveChangesAsync();
            User user = await context.Users
                      .Include(u => u.Avatar)
                      .Include(u => u.Role)
                      .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            ChangePasswordViewModel changePassModel = new ChangePasswordViewModel();
            var list = Enumerable.Reverse(await context.Users
                .Include(u => u.Avatar)
                .Include(u => u.Role).ToListAsync()).Take(5).ToList();
            var list2 = Enumerable.Reverse(await context.Threads
                .Include(u => u.Autor)
                .ToListAsync()).Take(5).ToList();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> NewComment(int id, string comment)
        {
            if (comment != null)
            {
                Threads th = await context.Threads
                        .Include(u => u.Autor)
                        .Include(u => u.Autor.Avatar)
                        .Include(r => r.Autor.Role)
                        .FirstOrDefaultAsync(i => i.Id == id);
                User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
                Comment com = new Comment
                {
                    Data = DateTime.Now,
                    TextComment = comment,
                    Thread = th,
                    Users = user
                };
                th.LastUpdate = DateTime.Now;
                context.Comments.Add(com);
                context.SaveChanges();
            }
            return RedirectToActionPermanent("thread", "thread", new { id = id});
        }

        public async Task<IActionResult> Articles() 
        {

            return View();
        }
    }
}
