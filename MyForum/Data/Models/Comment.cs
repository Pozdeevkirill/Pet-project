using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyForum.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public Threads Thread { get; set; }
        public User Users { get; set; }
        public DateTime Data { get; set; }
        public string TextComment { get; set; }
    }
}
