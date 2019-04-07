using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Usertoken
    {
        public int UserId { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual User User { get; set; }
    }
}
