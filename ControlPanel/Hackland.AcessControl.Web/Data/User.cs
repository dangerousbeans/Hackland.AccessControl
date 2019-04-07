using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class User : IdentityUser<int>
    {
        public User()
        {
            DoorCreatedByUser = new HashSet<Door>();
            DoorUpdatedByUser = new HashSet<Door>();
            PersonCreatedByUser = new HashSet<Person>();
            PersonUpdatedByUser = new HashSet<Person>();
            PersondoorCreatedByUser = new HashSet<PersonDoor>();
            PersondoorUpdatedByUser = new HashSet<PersonDoor>();
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Door> DoorCreatedByUser { get; set; }
        public virtual ICollection<Door> DoorUpdatedByUser { get; set; }
        public virtual ICollection<Person> PersonCreatedByUser { get; set; }
        public virtual ICollection<Person> PersonUpdatedByUser { get; set; }
        public virtual ICollection<PersonDoor> PersondoorCreatedByUser { get; set; }
        public virtual ICollection<PersonDoor> PersondoorUpdatedByUser { get; set; }
    }
}
