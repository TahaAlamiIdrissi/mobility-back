using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models
{
    public static class ModelBuilderSeed
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.Entity<IdentityUser>().HasData(
                new IdentityUser() { UserName = "seed", PasswordHash = "seed", Email = "seed@seed.com", EmailConfirmed = true }
            );
            builder.Entity<IdentityUser>().HasIndex(u => u.Email).IsUnique();
            builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "ADMIN_ID", Name = "Admin", NormalizedName = "Admin".ToUpper() });

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "STUDENT_ID", Name = "Student", NormalizedName = "Sudent".ToUpper() });
            
        }
    }
}