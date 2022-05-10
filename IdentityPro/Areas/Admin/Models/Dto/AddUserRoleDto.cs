using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Areas.Admin.Models.Dto
{
    public class AddUserRoleDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IdUser { get; set; }
        public string Role { get; set; }
        public List<SelectListItem> Roles { get; set; }
    }
}
