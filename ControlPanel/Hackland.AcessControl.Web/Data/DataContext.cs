using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Hackland.AccessControl.Web.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Door> Door { get; set; }
        public virtual DbSet<Doorread> Doorread { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Persondoor> Persondoor { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Roleclaim> Roleclaim { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Userclaim> Userclaim { get; set; }
        public virtual DbSet<Userlogin> Userlogin { get; set; }
        public virtual DbSet<Userrole> Userrole { get; set; }
        public virtual DbSet<Usertoken> Usertoken { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;database=accesscontrol;user=accesscontrol;password=accesscontrol;GuidFormat=LittleEndianBinary16;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Door>(entity =>
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

            modelBuilder.Entity<Doorread>(entity =>
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
                    .WithMany(p => p.Doorread)
                    .HasForeignKey(d => d.DoorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DoorRead_Door");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Doorread)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_DoorRead_Person");
            });

            modelBuilder.Entity<Person>(entity =>
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

            modelBuilder.Entity<Persondoor>(entity =>
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
                    .WithMany(p => p.Persondoor)
                    .HasForeignKey(d => d.DoorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonDoor_Door");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Persondoor)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonDoor_Person");

                entity.HasOne(d => d.UpdatedByUser)
                    .WithMany(p => p.PersondoorUpdatedByUser)
                    .HasForeignKey(d => d.UpdatedByUserId)
                    .HasConstraintName("FK_PersonDoor_UpdatedUser");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ConcurrencyStamp).HasColumnType("longtext");

                entity.Property(e => e.Name).HasColumnType("varchar(128)");

                entity.Property(e => e.NormalizedName).HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<Roleclaim>(entity =>
            {
                entity.ToTable("roleclaim");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_RoleClaim_RoleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClaimType).HasColumnType("longtext");

                entity.Property(e => e.ClaimValue).HasColumnType("longtext");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Roleclaim)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_RoleClaim_Role_RoleId");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");

                entity.Property(e => e.ConcurrencyStamp).HasColumnType("longtext");

                entity.Property(e => e.Email).HasColumnType("varchar(128)");

                entity.Property(e => e.EmailConfirmed).HasColumnType("bit(1)");

                entity.Property(e => e.FirstName).HasColumnType("longtext");

                entity.Property(e => e.LastName).HasColumnType("longtext");

                entity.Property(e => e.LockoutEnabled).HasColumnType("bit(1)");

                entity.Property(e => e.NormalizedEmail).HasColumnType("varchar(128)");

                entity.Property(e => e.NormalizedUserName).HasColumnType("varchar(128)");

                entity.Property(e => e.PasswordHash).HasColumnType("longtext");

                entity.Property(e => e.PhoneNumber).HasColumnType("longtext");

                entity.Property(e => e.PhoneNumberConfirmed).HasColumnType("bit(1)");

                entity.Property(e => e.SecurityStamp).HasColumnType("longtext");

                entity.Property(e => e.TwoFactorEnabled).HasColumnType("bit(1)");

                entity.Property(e => e.UserName).HasColumnType("varchar(128)");
            });

            modelBuilder.Entity<Userclaim>(entity =>
            {
                entity.ToTable("userclaim");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserClaim_UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ClaimType).HasColumnType("longtext");

                entity.Property(e => e.ClaimValue).HasColumnType("longtext");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userclaim)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserClaim_User_UserId");
            });

            modelBuilder.Entity<Userlogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PRIMARY");

                entity.ToTable("userlogin");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_UserLogin_UserId");

                entity.Property(e => e.LoginProvider).HasColumnType("varchar(128)");

                entity.Property(e => e.ProviderKey).HasColumnType("varchar(128)");

                entity.Property(e => e.ProviderDisplayName).HasColumnType("longtext");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userlogin)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserLogin_User_UserId");
            });

            modelBuilder.Entity<Userrole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PRIMARY");

                entity.ToTable("userrole");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_UserRole_RoleId");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.RoleId).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserRole_Role_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userrole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserRole_User_UserId");
            });

            modelBuilder.Entity<Usertoken>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("usertoken");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.LoginProvider)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Value).HasColumnType("longtext");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Usertoken)
                    .HasForeignKey<Usertoken>(d => d.UserId)
                    .HasConstraintName("FK_UserToken_User_UserId");
            });
        }
    }
}
