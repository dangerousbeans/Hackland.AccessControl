using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class AssociateDoorRequestViewModel
    {
        public int DoorReadId { get; set; }
        public int DoorId { get; set; }
        public string DoorName { get; set; }
        public List<SelectListItem> AvailablePeople { get; set; }
        public DateTime Timestamp { get; set; }
        public string TokenValue { get; set; }
    }
}
