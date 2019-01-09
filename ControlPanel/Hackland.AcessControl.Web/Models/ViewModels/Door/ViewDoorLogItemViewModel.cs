using Hackland.AccessControl.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class ViewDoorLogItemViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public ViewDoorLogItemPersonViewModel Person { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsTokenStillValid { get; set; }
        public bool IsTokenReallocated { get; set; }
        public ViewDoorLogItemPersonViewModel TokenReallocatedTo { get; set; }
        public bool IsTokenUnallocated { get; set; }
        public string TokenValue { get; internal set; }
    }
}
