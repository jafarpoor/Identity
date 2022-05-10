using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Models.Dto
{
    public class BlogDto
    {
        [BindNever]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        [BindNever]
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
