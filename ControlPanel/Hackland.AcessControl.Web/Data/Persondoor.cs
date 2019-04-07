using Hackland.AccessControl.Interfaces;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Persondoor : IMetadataFields
    {
        public int PersonId { get; set; }
        public int DoorId { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public int? UpdatedByUserId { get; set; }
        public short IsDeleted { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual Door Door { get; set; }
        public virtual Person Person { get; set; }
        public virtual User UpdatedByUser { get; set; }
    }
}
