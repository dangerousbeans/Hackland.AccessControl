using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hackland.AccessControl.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        protected DataContext DataContext { get; set; }

        public ApiController(DataContext dataContext)
        {
            this.DataContext = dataContext;
        }

        [HttpPost("door/register")]
        public ActionResult<bool> RegisterDoor(RegisterDoorModel model)
        {
            if (model == null) return Json(false);
            if (string.IsNullOrEmpty(model.MacAddress)) return Json(false);

            var door = DataContext.Doors.FirstOrDefault(d => d.MacAddress == model.MacAddress);
            if (door == null)
            {
                door = new Door()
                {
                    MacAddress = model.MacAddress,
                    Name = "Unknown",
                    Status = Data.Enums.DoorStatus.Unknown,
                    CreatedTimestamp = DateTime.Now
                };
                DataContext.Add(door);
            }
            door.LastHeartbeatTimestamp = DateTime.Now;
            door.IsDeleted = false;
            DataContext.SaveChanges();
            return Json(true);
        }

    }
}
