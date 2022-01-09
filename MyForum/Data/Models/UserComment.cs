using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.Data.Models
{
    public class UserComment
    {
        public int Id { get; set; }
        public User User { get; set; }
        public User Autor { get; set; }
        public DateTime Date { get; set; }
        public string comment { get; set; }
    }
}
