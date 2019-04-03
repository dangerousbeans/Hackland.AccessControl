using Hackland.AccessControl.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Data
{
    public class Door : IMetadataFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public DateTime LastHeartbeatTimestamp { get; set; }
        public DateTime? LastReadTimestamp { get; set; }
        public DoorStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public int? RemoteUnlockRequestSeconds { get; set; }

        public List<PersonDoor> PersonDoors { get; set; }
        public List<DoorRead> DoorReads { get; set; }

        public DateTime CreatedTimestamp { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
    }
}
