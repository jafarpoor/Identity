using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Dto
{
    public class VerifyPhoneNumberDto
    {
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public string Code { get; set; }
    }
}
