using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.Api
{
    public class ValidateDoorUnlockRequestModel
    {
        public string MacAddress { get; set; }
        public string TokenValue { get; set; }
    }
}
