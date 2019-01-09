using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Hackland.AccessControl.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Door> Doors { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<DoorRead> DoorReads { get; set; }
        public DbSet<PersonDoor> PeopleDoors { get; set; }

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
            builder.Entity<Person>(e => e.ToTable("Person"));
            builder.Entity<PersonDoor>(e => e.ToTable("PersonDoor"));
            builder.Entity<DoorRead>(e => e.ToTable("DoorRead"));

            builder.Entity<PersonDoor>()
                .HasKey(t => new { t.PersonId, t.DoorId });

            builder.Entity<PersonDoor>()
                .HasOne(pd => pd.Person)
                .WithMany(p => p.PersonDoors)
                .HasForeignKey(pd => pd.PersonId);

            builder.Entity<PersonDoor>()
                .HasOne(pd => pd.Door)
                .WithMany(p => p.PersonDoors)
                .HasForeignKey(pd => pd.DoorId);


            builder.Entity<DoorRead>()
                .HasKey(t => t.Id);

            builder.Entity<DoorRead>()
                .HasOne(pd => pd.Person)
                .WithMany(p => p.DoorReads)
                .HasForeignKey(pd => pd.PersonId);

            builder.Entity<DoorRead>()
                .HasOne(pd => pd.Door)
                .WithMany(p => p.DoorReads)
                .HasForeignKey(pd => pd.DoorId);
        }

    }
}
