using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            binding.ConvertTo<Door>(item);

            BindMetadataFields(item, binding.Mode);

            DataContext.SaveChanges();

            AddSuccess("Success", "Updated door {0}", item.Name);
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

            var recentItems = item.DoorReads
                .OrderByDescending(dr => dr.Timestamp)
                .Take(20)
                .Select(dr => new ViewDoorLogItemViewModel
                {
                    Id = dr.Id,
                    IsSuccess = dr.IsSuccess,
                    Timestamp = dr.Timestamp,
                    Person = dr.Person != null ? new Models.Api.ViewDoorLogItemPersonViewModel
                    {
                        Id = dr.Person.Id,
                        EmailAddress = dr.Person.EmailAddress,
                        Name = dr.Person.Name
                    } : null
                })
                .ToList();

            model.RecentItems = recentItems;

            return View(model);

        }

    }
}
