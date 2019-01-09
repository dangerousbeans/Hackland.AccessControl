using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.Api
{
    public class ValidateDoorUnlockResponseModel
    {
        public bool IsUnlockAllowed { get; set; }
        public string Message { get; set; }
        public int? DoorReadId { get; set; }
        public ValidateDoorUnlockPersonModel MatchedPerson { get; set; }
    }
}
