using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Door
    {
        public Door()
        {
            Doorread = new HashSet<Doorread>();
            Persondoor = new HashSet<Persondoor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public DateTime LastHeartbeatTimestamp { get; set; }
        public DateTime? LastReadTimestamp { get; set; }
        public int Status { get; set; }
        public short IsDeleted { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? UpdatedTimestamp { get; set; }
        public int? UpdatedByUserId { get; set; }
        public int? RemoteUnlockRequestSeconds { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public virtual ICollection<Doorread> Doorread { get; set; }
        public virtual ICollection<Persondoor> Persondoor { get; set; }
    }
}
