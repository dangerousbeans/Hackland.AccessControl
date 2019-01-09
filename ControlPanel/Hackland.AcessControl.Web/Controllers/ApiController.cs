using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

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

        [HttpPost("door/validate")]
        public ActionResult<ValidateDoorUnlockResponseModel> Validate(ValidateDoorUnlockRequestModel model)
        {
            if (model == null) return Json(new ValidateDoorUnlockResponseModel { IsUnlockAllowed = false, Message = "No model provided" });
            if (string.IsNullOrEmpty(model.MacAddress)) return Json(new ValidateDoorUnlockResponseModel { IsUnlockAllowed = false, Message = "No device mac address provided" });
            if (string.IsNullOrEmpty(model.TokenValue)) return Json(new ValidateDoorUnlockResponseModel { IsUnlockAllowed = false, Message = "No scan token provided" });

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var door = DataContext.Doors.FirstOrDefault(d => d.MacAddress == model.MacAddress);
                if (door == null)
                {
                    return Json(new ValidateDoorUnlockResponseModel { IsUnlockAllowed = false, Message = "Invalid door mac address" });
                }

                if (door.IsDeleted)
                {
                    return Json(new ValidateDoorUnlockResponseModel { IsUnlockAllowed = false, Message = "Invalid door" });
                }

                door.LastReadTimestamp = DateTime.Now;

                var person = DataContext.People
                    .Include(p => p.PersonDoors)
                    .FirstOrDefault(p => p.TokenValue == model.TokenValue);

                bool isAccessAllowed = person != null &&
                    !person.IsDeleted &&
                    person.PersonDoors != null &&
                    person.PersonDoors.Any(pd => pd.DoorId == door.Id && !pd.IsDeleted);

                var read = new DoorRead
                {
                    IsSuccess = isAccessAllowed,
                    DoorId = door.Id,
                    PersonId = person != null ? person.Id : (int?)null,
                    Timestamp = DateTime.Now,
                    TokenValue = model.TokenValue
                };
                DataContext.DoorReads.Add(read);
                DataContext.SaveChanges();

                var doorReadId = read.Id;

                if (person == null)
                {
                    return Json(new ValidateDoorUnlockResponseModel
                    {
                        DoorReadId = doorReadId,
                        IsUnlockAllowed = false,
                        Message = "Token does not match a person"
                    });
                }
                else
                {
                    return Json(new ValidateDoorUnlockResponseModel
                    {
                        DoorReadId = doorReadId,
                        IsUnlockAllowed = isAccessAllowed,
                        MatchedPerson = person.ConvertTo<ValidateDoorUnlockPersonModel>(),
                        Message = "Success"
                    });
                }

            }
        }


    }
}
