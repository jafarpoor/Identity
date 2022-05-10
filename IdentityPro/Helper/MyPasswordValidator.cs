using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Helper
{
    public class MyPasswordValidator : IPasswordValidator<User>
    {

        List<string> CommonPassword = new List<string>()
        {
            "123456","zxcV@34567","password","qwerty","123456789"
        };
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            if (CommonPassword.Contains(password))
            {
                var result = IdentityResult.Failed(new IdentityError
                {
                    Code = "CommonPassword",
                    Description = "پسورد قوی تر انتخاب کنید"
                });

                return Task.FromResult(result);
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
