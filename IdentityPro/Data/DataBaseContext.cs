using IdentityPro.Models.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityPro.Data
{
    public class DataBaseContext :IdentityDbContext<User , Role , string>
    {

        public DataBaseContext(DbContextOptions<DataBaseContext> options ) :base(options)
        {
            
        }
        public DbSet<Blog> Blogs { get; set; }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<IdentityUserLogin<string>>().HasKey(p => new {p.ProviderKey , p.LoginProvider });
        //    builder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider });
        //    builder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });

        //    builder.Entity<User>().Ignore(p => p.NormalizedEmail);
        //}
    }
}
