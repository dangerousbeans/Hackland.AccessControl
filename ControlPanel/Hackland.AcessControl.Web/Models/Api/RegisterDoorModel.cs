using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.Api
{
    public class RegisterDoorModel
    {
        public string MacAddress { get; set; }
        public bool LockTriggerStatus { get; set; }
        public bool LockReedStatus { get; set; }
        public bool LockMagBondStatus { get; set; }
        public bool LockRequestExitStatus { get; set; }
    }
}
