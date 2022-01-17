using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForum.Data.Models;
using MyForum.ViewModels;
using MyForum.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using File = Data.Models.File;

namespace MyForum.Controllers
{
    public class AccountController : Controller
    {
        private ForumDBContext context;
        IWebHostEnvironment appEnvironment;

        public AccountController(ForumDBContext _context, IWebHostEnvironment _appEnvironment)
        {
            context = _context;
            appEnvironment = _appEnvironment;
        }
        #region Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View();
        }
        #endregion
        #region Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await context.Users.FirstOrDefaultAsync(u => u.Name == model.Name);
                if (user == null)
                {
                    User user2 = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (user2 == null)
                    {
                        // добавляем пользователя в бд
                        user = new User { Email = model.Email, Password = model.Password, Name = model.Name };
                        Role userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                        if (userRole != null)
                            user.Role = userRole;
                        File DefaultAvatar = new File { Name = user.Name, Path = "/Files/Images/Avatar/no_avatar.png " };
                        user.Avatar = DefaultAvatar;
                        user.RegisterData = DateTime.Now;
                        context.Users.Add(user);
                        await context.SaveChangesAsync();

                        await Authenticate(user); // аутентификация

                        return RedirectToAction("Index", "Home", model);
                    }
                    else
                        ModelState.AddModelError("", $"На {user2.Email} уже зарегестрирован аккаунт");
                }
                else
                    ModelState.AddModelError("", $"Имя пользователя {user.Name} уже занято");
            }
            return View();
        }
        #endregion
        #region Authenticate
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                new Claim (ClaimsIdentity.DefaultIssuer, user.Email)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        #endregion
        #region Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        #endregion
        #region Profile
        [HttpGet]
        [Authorize]
        [Authorize(Roles = "admin, user, moder")]
        public async Task<IActionResult> Profile()
        {
            User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            ChangePasswordViewModel changePassModel = new ChangePasswordViewModel();
            if (user != null)
                return View(Tuple.Create(user, changePassModel));
            else
                return Content("Ooops... Страница не найдена :(");
        }
        #endregion
        #region ProfileSetting
        #region ChangeStatus
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "admin, user, moder")]
        public async Task<IActionResult> ChangeStatus(string status)
        {
            User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            user.Status = status;
            context.Update(user);
            context.SaveChanges();
            return Redirect("Profile");
        }
        #endregion
        #region EditAvatar
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPhoto(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
                if (user.Avatar != null)
                {
                    //Путь к Аватарке
                    string path = "/Files/Images/Avatar/" + uploadedFile.FileName;
                    //Сохранение аватарки
                    using (var fileStream = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    user.Avatar.Path = path;
                    context.Users.Update(user);
                    context.SaveChanges();
                }
                else
                {
                    string path = "/Files/Images/Avatar/" + uploadedFile.FileName;
                    //Сохранение аватарки в папку wwwroot/Files/Avatar
                    using (var fileStream = new FileStream(appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    File avatar = new File { Name = User.Identity.Name, Path = path };
                    user.Avatar = avatar;
                    context.Files.Update(avatar);
                    context.Users.Update(user);
                    context.SaveChanges();
                }
            }
            return Redirect("Profile");
        }
        #endregion
        #region ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurentPassword, string Password, string ConfirmPassword)
        {
            User user = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            if (CurentPassword == user.Password)
            {

            }
            else
            {
                return Error("Не правельно введн текущий пароль!");
            }
            return Ok();
        }
        #endregion
        #endregion
        #region User
        public async Task<IActionResult> UserView(int id)
        {
            
            User userView = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Id == id);
            User userIdentity = await context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Avatar)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
            //var comments = Enumerable.Reverse(await context.UserComments
            //   .Include(u => u.User)
            //   .Include(u => u.User.Avatar)
            //   .Include(u => u.User.Role).ToListAsync());
            var comments = Enumerable.Reverse(await context.UserComments
                .Include(u => u.Autor)
                .Include(u => u.Autor.Avatar)
                .Include(u => u.Autor.Role)
                .Include(u => u.User).ToListAsync()).ToList();
            comments.RemoveAll(u => u.User != userView);
            if (User.Identity.Name != userView.Name) 
            {
                return View(Tuple.Create(userView,userIdentity,comments));
            }
            else
            {
                return RedirectToAction("Profile");
            }

            
        }
        public async Task<IActionResult> AddUserComment(int id, string comment)
        {
            if (comment != null)
            {
                User user = await context.Users //Пользователь, которому отправляют комментарий
                    .Include(u => u.Avatar)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
                User autor = await context.Users //Пользователь, который отправляет комментарий
                    .Include(u => u.Avatar)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Name == User.Identity.Name);
                UserComment com = new UserComment
                {
                    User = user,
                    Autor = autor,
                    Date = DateTime.Now,
                    comment = comment
                };

                context.UserComments.Add(com);
                context.SaveChanges();

            }
            return RedirectToActionPermanent("UserView", "Account", new { id = id });
        }
        #endregion
        [HttpGet]
        public IActionResult Error(string msg)
        {
            return View(msg);
        }
    }
}
