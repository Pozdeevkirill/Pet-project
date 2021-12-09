using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.ViewModels.Account
{
    public class ProfileViewModel
    {
        #region User
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public File Avatar { get; set; }
        #endregion
        #region ChangePass
        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string CurentPassword { get; set; }
        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно.")]
        public string ConfirmNewPassword { get; set; }
        #endregion
    }
}
