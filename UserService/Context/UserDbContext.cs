using System.Reflection.Emit;
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

        public DbSet<RefreshToken> RefreshTokens { get; set; }

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

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }

    }
}
