using Hackland.AccessControl.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Data
{
    public class PersonDoor : IMetadataFields
    {
        public int PersonId { get; set; }
        public int DoorId { get; set; }

        public DateTime CreatedTimestamp { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Person Person { get; set; }
        public virtual Door Door { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }

    }
}
