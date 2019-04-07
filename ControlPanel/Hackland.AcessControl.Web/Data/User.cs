using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class User
    {
        public User()
        {
            DoorCreatedByUser = new HashSet<Door>();
            DoorUpdatedByUser = new HashSet<Door>();
            PersonCreatedByUser = new HashSet<Person>();
            PersonUpdatedByUser = new HashSet<Person>();
            PersondoorCreatedByUser = new HashSet<Persondoor>();
            PersondoorUpdatedByUser = new HashSet<Persondoor>();
            Userclaim = new HashSet<Userclaim>();
            Userlogin = new HashSet<Userlogin>();
            Userrole = new HashSet<Userrole>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual Usertoken Usertoken { get; set; }
        public virtual ICollection<Door> DoorCreatedByUser { get; set; }
        public virtual ICollection<Door> DoorUpdatedByUser { get; set; }
        public virtual ICollection<Person> PersonCreatedByUser { get; set; }
        public virtual ICollection<Person> PersonUpdatedByUser { get; set; }
        public virtual ICollection<Persondoor> PersondoorCreatedByUser { get; set; }
        public virtual ICollection<Persondoor> PersondoorUpdatedByUser { get; set; }
        public virtual ICollection<Userclaim> Userclaim { get; set; }
        public virtual ICollection<Userlogin> Userlogin { get; set; }
        public virtual ICollection<Userrole> Userrole { get; set; }
    }
}
