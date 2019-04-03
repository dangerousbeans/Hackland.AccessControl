using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models;
using Hackland.AccessControl.Web.Models.Api;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hackland.AccessControl.Web.Controllers
{
    [Authorize]
    public class DoorController : ControllerBase
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

        [HttpGet]
        public IActionResult Update(int id)
        {
            var item = DataContext.Doors
                .Include(door => door.DoorReads)
                .ThenInclude(personDoors => personDoors.Person)
                .Where(d => d.Id == id)
                .Select(d => d)
                .FirstOrDefault();

            if (item == null)
            {
                return RedirectToAction("Index");
            }

            var model = item.ConvertTo<UpdateDoorViewModel>();
            model.Mode = CreateUpdateModeEnum.Update;

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdateDoorBindingModel binding)
        {
            if (!ModelState.IsValid)
            {
                return View(binding.ConvertTo<UpdateDoorViewModel>());
            }
            var item = DataContext.Doors
             .Where(p => p.Id == binding.Id)
             .Select(p => p)
             .FirstOrDefault();

            string oldName = item.Name;
            if (!string.Equals(binding.Name, oldName))
            {
                binding.ConvertTo<Door>(item);

                BindMetadataFields(item, binding.Mode);

                DataContext.SaveChanges();

                AddSuccess("Success", "Updated door name from {0} to {1}", oldName, binding.Name);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = DataContext.Doors
                .Include(d => d.PersonDoors)
                .Where(p => p.Id == id)
                .Select(p => p)
                .FirstOrDefault();

            item.IsDeleted = true;
            BindMetadataFields(item, CreateUpdateModeEnum.Update);
            item.PersonDoors.ForEach(pd => BindMetadataFields(pd, CreateUpdateModeEnum.Update).IsDeleted = true);

            DataContext.SaveChanges();

            AddSuccess("Success", "Deleted door {0}", item.Name);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Log(int id)
        {
            var item = DataContext.Doors
                .Include(d => d.DoorReads)
                .ThenInclude(dr => dr.Person)
                .Where(p => p.Id == id)
                .Select(p => p)
                .FirstOrDefault();

            var model = item.ConvertTo<ViewDoorLogViewModel>();

            var recentItems = (from dr in DataContext.DoorReads
                               where dr.DoorId == item.Id
                               join person in DataContext.People on dr.PersonId equals person.Id into person_loj
                               from person in person_loj.DefaultIfEmpty()
                               orderby dr.Timestamp descending
                               select new ViewDoorLogItemViewModel
                               {
                                   Id = dr.Id,
                                   IsSuccess = dr.IsSuccess,
                                   Timestamp = dr.Timestamp,
                                   TokenValue = dr.TokenValue,
                                   Person = person != null ? new ViewDoorLogItemPersonViewModel
                                   {
                                       Id = person.Id,
                                       EmailAddress = person.EmailAddress,
                                       Name = person.Name,
                                       IsDeleted = person.IsDeleted
                                   } : null
                               })
                .ToList();

            //this is kinda bad (N+1 query) but ef core is not mature and let expressions are failing
            foreach (var read in recentItems)
            {
                var tokenValue = read.TokenValue;
                var tokenCurrentlyAllocatedTo = DataContext.People.FirstOrDefault(ip => ip.TokenValue == tokenValue);

                if (tokenCurrentlyAllocatedTo == null)
                {
                    //token is not currently assigned to anyone
                    read.IsTokenUnallocated = true;
                    read.IsTokenStillValid = false;
                    read.IsTokenReallocated = false;
                }
                else
                {
                    //token is currently assigned to someone
                    if (read.Person != null && tokenCurrentlyAllocatedTo.Id == read.Person.Id)
                    {
                        //same person
                        read.IsTokenReallocated = false;
                        read.IsTokenStillValid = true;
                        read.IsTokenUnallocated = false;
                    }
                    else
                    {
                        if (read.Person == null)
                        {
                            read.IsTokenReallocated = false;
                            read.IsTokenStillValid = false;
                            read.IsTokenUnallocated = false;
                        }
                        else
                        {
                            read.IsTokenReallocated = true;
                            read.IsTokenStillValid = false;
                            read.IsTokenUnallocated = false;
                            read.TokenReallocatedTo = tokenCurrentlyAllocatedTo.ConvertTo<ViewDoorLogItemPersonViewModel>();
                        }
                    }
                }
            }

            model.RecentItems = recentItems;

            return View(model);

        }

        [HttpGet]
        [Route("door/remote-unlock/{id}")]
        public IActionResult RemoteUnlock(int id)
        {
            var item = DataContext.Doors
                .Where(p => p.Id == id)
                .FirstOrDefault();

            item.RemoteUnlockRequestSeconds = 60;

            DataContext.SaveChanges();

            AddSuccess("Success", "Requested remote unlock of door {0}", item.Name);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Disassociate(int id)
        {
            var read = DataContext.DoorReads
                .Include(dr => dr.Person)
                .FirstOrDefault(dr => dr.Id == id);

            if (read == null)
            {
                return RedirectToAction("Index");
            }

            read.Person.TokenValue = null;

            DataContext.SaveChanges();

            AddSuccess("Dissociated", "Cleared access token stored against {0}", read.Person.Name);

            return RedirectToAction("Log", new { Id = read.DoorId });

        }

        [HttpGet]
        public IActionResult Assign(int id)
        {
            var read = DataContext.DoorReads
                .Include(dr => dr.Person)
                .Include(dr => dr.Door)
                .FirstOrDefault(dr => dr.Id == id);

            if (read == null)
            {
                return RedirectToAction("Index");
            }

            AssociateDoorRequestViewModel model = GetAssociateDoorRequestModel(read);
            return View(model);

        }

        private AssociateDoorRequestViewModel GetAssociateDoorRequestModel(DoorRead read)
        {
            return new AssociateDoorRequestViewModel
            {
                DoorReadId = read.Id,
                DoorId = read.DoorId,
                Timestamp = read.Timestamp,
                DoorName = read.Door.Name == "Unknown" ? read.Door.MacAddress : read.Door.Name,
                TokenValue = read.TokenValue,
                AvailablePeople = DataContext.People
                                .Where(p => !p.IsDeleted && (p.TokenValue == null || p.TokenValue == string.Empty))
                                .OrderBy(p => p.Name)
                                .Select(p => new SelectListItem
                                {
                                    Text = p.Name,
                                    Value = p.Id.ToString()
                                })
                                .ToList()
            };
        }

        [HttpPost]
        public IActionResult Assign(AssociateDoorResponseViewModel model)
        {
            var person = DataContext.People.FirstOrDefault(p => p.Id == model.PersonId);
            var read = DataContext.DoorReads.Include(dr => dr.Door).Include(dr => dr.Person).First(dr => dr.Id == model.DoorReadId);
            if (!ModelState.IsValid)
            {
                AssociateDoorRequestViewModel requestModel = GetAssociateDoorRequestModel(read);
                return View(requestModel);
            }

            //set token value on the person
            person.TokenValue = read.TokenValue;
            //set the person on the read
            read.Person = person;

            DataContext.SaveChanges();

            AddSuccess("Associated", "Assigned access token to {0}", read.Person.Name);

            return RedirectToAction("Log", new { Id = read.DoorId });
        }
    }
}
