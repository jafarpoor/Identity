using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Entites
{
    public class Blog
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
