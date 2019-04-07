using Hackland.AccessControl.Interfaces;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Person : IMetadataFields
    {
        public Person()
        {
            DoorReads = new HashSet<DoorRead>();
            PersonDoors = new HashSet<PersonDoor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime? LastSeenTimestamp { get; set; }
        public bool IsDeleted { get; set; }
        public string TokenValue { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public virtual ICollection<DoorRead> DoorReads { get; set; }
        public virtual ICollection<PersonDoor> PersonDoors { get; set; }
    }
}
