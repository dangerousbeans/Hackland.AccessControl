using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.Api
{
    public class RegisterDoorResponseModel
    {
        public int DoorId { get; set; }
        public bool Success { get; set; }
        public int RemoteUnlockRequestSeconds { get; set; }
    }
}
