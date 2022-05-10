using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Dto
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Remmber me")]
        public bool IsPersistent { get; set; }

        public string ReturnUrl { get; set; }
    }
}
