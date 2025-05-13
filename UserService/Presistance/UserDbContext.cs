using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Consts;
using UserService.Entities;

namespace UserService.Presistance
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
               new IdentityRole
               {
                   Id = "1", // Ensure unique string
                   Name = RoleName.Admin,
                   NormalizedName = RoleName.Admin.ToUpper()
               },
               new IdentityRole
               {
                   Id = "2",
                   Name = RoleName.User,
                   NormalizedName = RoleName.User.ToUpper()
               }
           );
     
        }

    }
}
