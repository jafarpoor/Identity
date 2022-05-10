using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Helper
{
    public class UserCreditRequerment : IAuthorizationRequirement
    {
        public int Credit { get; set; }

        public UserCreditRequerment(int credit)
        {
            Credit = credit;
        }
    }

    public class UserCreditHandler : AuthorizationHandler<UserCreditRequerment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCreditRequerment requirement)
        {
            var cliam = context.User.FindFirst("Credit");
            if (cliam != null)
            {
                int cliamValue = int.Parse(cliam.Value);
                if (cliamValue >= requirement.Credit)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
