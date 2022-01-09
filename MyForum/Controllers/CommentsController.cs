using Data;
using Data.Models;
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
    public class CommentsController : Controller
    {
        private ForumDBContext context;
        public CommentsController(ForumDBContext _context)
        {
            context = _context;
        }

        //public async Task<IActionResult> NewComment(int id, string comment)
        //{
        //    if(comment != null)
        //    {
        //        Threads th = await context.Threads
        //                .Include(u => u.Autor)
        //                .Include(u => u.Autor.Avatar)
        //                .Include(r => r.Autor.Role)
        //                .FirstOrDefaultAsync(i => i.Id == id);
        //        User user = await context.Users
        //            .Include(u => u.Role)
        //            .Include(u => u.Avatar)
        //            .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
        //        Comment com = new Comment
        //        {
        //            Data = DateTime.Now,
        //            TextComment = comment,
        //            Thread = th,
        //            Users = user
        //        };
                
                
        //        var com1 = Enumerable.Reverse(await context.Comments
        //           .Include(u => u.Users)
        //           .Include(u => u.Users.Avatar)
        //           .Include(u => u.Users.Role)
        //           .Include(th => th.Thread).ToListAsync()).ToList();
        //        th.LastUpdate = DateTime.Now;
        //        context.Comments.Add(com);
        //        context.SaveChanges();
        //        return RedirectToAction("thread","thread", id);
        //    }
        //    return View();
        //}
    }
}
