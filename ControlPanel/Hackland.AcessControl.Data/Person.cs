using Hackland.AccessControl.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Data
{
    public class Person : IMetadataFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? LastSeenTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public List<PersonDoor> PersonDoors { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public List<DoorRead> DoorReads { get; set; }

        public string TokenValue { get; set; }
    }
}
