using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указано Имя.")]
        [MinLength(5,ErrorMessage ="Минимальное число символов: 5.")]
        [MaxLength(20, ErrorMessage ="Максимальное число символов: 20.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указан Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно.")]
        public string ConfirmPassword { get; set; }
    }
}
