using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Threads
    {
        public int Id { get; set; }
        public DateTime DataPublish { get; set; }
        public User Autor { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime LastUpdate { get; set; }
        public int CommentType { get; set; }
    }
}
