using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class DoorRead
    {
        public int Id { get; set; }
        public string TokenValue { get; set; }
        public DateTime? Timestamp { get; set; }
        public int DoorId { get; set; }
        public int? PersonId { get; set; }
        public short IsSuccess { get; set; }

        public virtual Door Door { get; set; }
        public virtual Person Person { get; set; }
    }
}
