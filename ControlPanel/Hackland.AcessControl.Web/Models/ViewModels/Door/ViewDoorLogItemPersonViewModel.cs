using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public class ViewDoorLogItemPersonViewModel
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
