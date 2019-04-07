using Hackland.AccessControl.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class DoorListViewModel
    {
        public IEnumerable<Door> Items { get; set; }
    }
}
