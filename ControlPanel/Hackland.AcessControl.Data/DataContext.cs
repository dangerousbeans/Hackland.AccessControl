using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hackland.AccessControl.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Door> Doors { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e => e.ToTable("User"));
            builder.Entity<Role>(e => e.ToTable("Role"));
            builder.Entity<IdentityUserClaim<Guid>>(e => e.ToTable("UserClaim"));
            builder.Entity<IdentityUserLogin<Guid>>(e => e.ToTable("UserLogin"));
            builder.Entity<IdentityUserRole<Guid>>(e => e.ToTable("UserRole"));
            builder.Entity<IdentityUserToken<Guid>>(e => e.ToTable("UserToken"));
            builder.Entity<IdentityRoleClaim<Guid>>(e => e.ToTable("RoleClaim"));
            builder.Entity<Door>(e => e.ToTable("Door"));
        }

    }
}
