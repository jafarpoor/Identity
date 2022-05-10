using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Entites
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
