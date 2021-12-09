using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.ViewModels
{
    public class ChangePasswordViewModel
    {

        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string CurentPassword { get; set; }
        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно.")]
        public string ConfirmPassword { get; set; }
    }
}
