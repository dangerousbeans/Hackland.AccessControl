using System;
using System.Collections.Generic;

namespace Hackland.AccessControl.Web.Data
{
    public partial class Userlogin
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
