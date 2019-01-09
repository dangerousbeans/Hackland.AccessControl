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
    }
}
