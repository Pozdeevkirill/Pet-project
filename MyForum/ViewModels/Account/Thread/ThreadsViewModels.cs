using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.ViewModels.Theme
{
    public class ThreadsViewModels
    {
        [Required(ErrorMessage = "Не указано название.")]
        [MinLength(5, ErrorMessage = "Минимальное число символов: 5.")]
        [MaxLength(45, ErrorMessage = "Максимальное число символов: 45.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Не указано содержимое темы.")]
        [MinLength(50, ErrorMessage = "Минимальное число символов: 50.")]
        public string Content { get; set; }
        public User Autor { get; set; }
    }
}
