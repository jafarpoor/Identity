using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Entites
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        public string FristName { get; set; }
        public string LastName { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }
}
