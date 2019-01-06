using Hackland.AccessControl.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Data
{
    public class DoorRead
    {
        public int Id { get; set; }

        public int? PersonId { get; set; }
        public int DoorId { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual Person Person { get; set; }
        public virtual Door Door { get; set; }

        public string TokenValue { get; set; }
        public bool IsSuccess { get; set; }
    }
}
