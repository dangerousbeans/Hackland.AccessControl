using System;
using Hackland.AccessControl.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hackland.AccessControl.Web.Data
{
    public partial class DataContext : IdentityDbContext<User, Role, int>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Door> Doors { get; set; }
        public virtual DbSet<DoorRead> DoorReads { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PersonDoor> PersonDoors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e => e.ToTable("User"));
            builder.Entity<Role>(e => e.ToTable("Role"));
            builder.Entity<IdentityUserClaim<int>>(e => e.ToTable("UserClaim"));
            builder.Entity<IdentityUserLogin<int>>(e => e.ToTable("UserLogin"));
            builder.Entity<IdentityUserRole<int>>(e => e.ToTable("UserRole"));
            builder.Entity<IdentityUserToken<int>>(e => e.ToTable("UserToken"));
            builder.Entity<IdentityRoleClaim<int>>(e => e.ToTable("RoleClaim"));

            builder.Entity<Door>(entity =>
            {
                entity.ToTable("door");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("FK_Door_CreatedUser");

                entity.HasIndex(e => e.UpdatedByUserId)
                    .HasName("FK_Door_UpdatedUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedByUserId).HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MacAddress)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.RemoteUnlockRequestSeconds).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.UpdatedByUserId).HasColumnType("int(11)");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.DoorCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_Door_CreatedUser");

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.DoorUpdatedByUser)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_Door_UpdatedUser");
            });

            builder.Entity<DoorRead>(entity =>
            {
                entity.ToTable("doorread");

                entity.HasIndex(e => e.DoorId)
                    .HasName("FK_DoorRead_Door");

                entity.HasIndex(e => e.PersonId)
                    .HasName("FK_DoorRead_Person");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.DoorId).HasColumnType("int(11)");

                entity.Property(e => e.IsSuccess).HasColumnType("smallint(6)");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.TokenValue)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.HasOne(d => d.Door)
                    .WithMany(p => p.DoorReads)
                    .HasForeignKey(d => d.DoorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoorRead_Door");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.DoorReads)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_DoorRead_Person");
            });

            builder.Entity<Person>(entity =>
            {
                entity.ToTable("person");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("FK_Person_CreatedUser");

                entity.HasIndex(e => e.UpdatedByUserId)
                    .HasName("FK_Person_UpdatedUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreatedByUserId).HasColumnType("int(11)");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.IsDeleted).HasColumnType("smallint(6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("longtext");

                entity.Property(e => e.PhoneNumber).HasColumnType("longtext");

                entity.Property(e => e.TokenValue).HasColumnType("longtext");

                entity.Property(e => e.UpdatedByUserId).HasColumnType("int(11)");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PersonCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Person_CreatedUser");

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.PersonUpdatedByUser)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_Person_UpdatedUser");
            });

            builder.Entity<PersonDoor>(entity =>
            {
                entity.HasKey(e => new { e.PersonId, e.DoorId })
                    .HasName("PRIMARY");

                entity.ToTable("persondoor");

                entity.HasIndex(e => e.CreatedByUserId)
                    .HasName("FK_PersonDoor_CreatedUser");

                entity.HasIndex(e => e.DoorId)
                    .HasName("FK_PersonDoor_Door");

                entity.HasIndex(e => e.UpdatedByUserId)
                    .HasName("FK_PersonDoor_UpdatedUser");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.DoorId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedByUserId).HasColumnType("int(11)");

                entity.Property(e => e.IsDeleted).HasColumnType("smallint(6)");

                entity.Property(e => e.UpdatedByUserId).HasColumnType("int(11)");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.PersondoorCreatedByUser)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonDoor_CreatedUser");

                entity.HasOne(d => d.Door)
                    .WithMany(p => p.PersonDoors)
                    .HasForeignKey(d => d.DoorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonDoor_Door");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonDoors)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonDoor_Person");

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.PersondoorUpdatedByUser)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_PersonDoor_UpdatedUser");
            });

            if (Settings.IsRunningInDocker || !Settings.UseSqlServer)
            {
                foreach (var entityType in builder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(bool))
                        {
                            property.SetValueConverter(new BoolToZeroOneConverter<Int16>());
                        }
                    }
                }
            }

        }
    }
}
