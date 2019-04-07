using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Roleclaim
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual Role Role { get; set; }
    }
}
