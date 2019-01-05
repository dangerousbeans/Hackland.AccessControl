using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Models;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

namespace Hackland.AccessControl.Web.Controllers
{
    [Authorize]
    public class DoorController : Controller
    {
        protected DataContext DataContext { get; set; }

        public DoorController(DataContext dataContext)
        {
            this.DataContext = dataContext;
        }

        public IActionResult Index()
        {
            var epoch = DateTime.Now.AddDays(-1);
            var expired = (from d in DataContext.Doors where !d.IsDeleted && d.LastHeartbeatTimestamp < epoch select d);
            if (expired.Any())
            {
                foreach (var expiredDoor in expired)
                {
                    expiredDoor.IsDeleted = true;
                }
                DataContext.SaveChanges();
            }

            var doors = (from d in DataContext.Doors where !d.IsDeleted select d);
            var model = new DoorListViewModel
            {
                Items = doors.ToList()
            };

            return View(model);
        }

    }
}
