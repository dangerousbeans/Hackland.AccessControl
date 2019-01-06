using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Hackland.AccessControl.Web.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        protected DataContext DataContext { get; set; }

        public PersonController(DataContext dataContext)
        {
            this.DataContext = dataContext;
        }

        public IActionResult Index()
        {
            var people = (from d in DataContext.People orderby d.Name where !d.IsDeleted select d);
            var doors = (from d in DataContext.Doors where !d.IsDeleted select d);
            //if there is a recorded scan which has not been linked to a person
            var isAssignAvailable = (from dr in DataContext.DoorReads where dr.PersonId == null select dr.Id).Any();
            var model = new PersonListViewModel
            {
                Items = people.ToList(),
                IsCreateAvailable = doors.Any(),
                IsAssignAvailable = isAssignAvailable
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //Pattern for these view models came from https://stackoverflow.com/questions/17107334/same-view-for-both-create-and-edit-in-mvc4/25374200
            var model = new CreatePersonViewModel();
            model.Mode = CreateUpdateModeEnum.Create;
            BindAvailableDoors(model);

            return View(model);
        }

        private void BindAvailableDoors(IPersonViewModel model)
        {
            model.Doors = (from d in DataContext.Doors where !d.IsDeleted select new SelectListItem(d.Name == "Unknown" ? d.MacAddress : d.Name, d.Id.ToString(), model.Mode == CreateUpdateModeEnum.Create ? true : false, false)).ToList();
        }

        [HttpPost]
        public IActionResult Create(CreatePersonBindingModel binding)
        {
            if (!ModelState.IsValid)
            {
                return View(binding.ConvertTo<CreatePersonViewModel>());
            }
            var item = binding.ConvertTo<Person>();
            BindMetadataFields(item, binding.Mode);
            SynchronisePersonDoor(binding, item);
            DataContext.People.Add(item);
            DataContext.SaveChanges();
            return RedirectToAction("Index");

        }

        private void SynchronisePersonDoor(IPersonViewModel binding, Person item)
        {
            var selected = binding.Doors.Where(d => d.Selected).Select(d => int.Parse(d.Value)).ToList();
            if (item.PersonDoors == null)
            {
                item.PersonDoors = new List<PersonDoor>();
            }
            //undelete
            foreach (var pd in item.PersonDoors.Where(pd => pd.IsDeleted && selected.Contains(pd.DoorId)))
            {
                pd.IsDeleted = false;
                BindMetadataFields(pd, CreateUpdateModeEnum.Update);
            }
            //delete
            foreach (var pd in item.PersonDoors.Where(pd => !pd.IsDeleted && !selected.Contains(pd.DoorId)))
            {
                pd.IsDeleted = true;
                BindMetadataFields(pd, CreateUpdateModeEnum.Update);
            }
            //add new
            item.PersonDoors.AddRange(selected
                .Except(item.PersonDoors != null ?
                    item.PersonDoors.Select(pd => pd.DoorId) :
                    Enumerable.Empty<int>())
                .Select(doorId => BindMetadataFields(new PersonDoor()
                {
                    DoorId = doorId
                }, CreateUpdateModeEnum.Create)));
        }

        private T BindMetadataFields<T>(T item, CreateUpdateModeEnum mode) where T : IMetadataFields
        {
            var userId = Guid.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (mode == CreateUpdateModeEnum.Create)
            {
                item.CreatedTimestamp = DateTime.Now;
                item.CreatedByUserId = userId;
            }
            else
            {
                item.UpdatedTimestamp = DateTime.Now;
                item.UpdatedByUserId = userId;
            }
            return item;
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var item = DataContext.People
                .Include(person => person.PersonDoors)
                .ThenInclude(personDoors => personDoors.Door)
                .Where(p => p.Id == id)
                .Select(p => p)
                .FirstOrDefault();

            if(item == null)
            {
                return RedirectToAction("Index");
            }

            var model = item.ConvertTo<UpdatePersonViewModel>();
            model.Mode = CreateUpdateModeEnum.Update;
            BindAvailableDoors(model);

            //copy selected state into doors
            if (item.PersonDoors != null)
            {
                foreach (var selectedDoor in item.PersonDoors.Where(pd => !pd.IsDeleted).Select(pd => pd.DoorId))
                {
                    var selected = model.Doors.FirstOrDefault(d => string.Equals(d.Value, selectedDoor.ToString()));
                    selected.Selected = true;
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdatePersonBindingModel binding)
        {
            if (!ModelState.IsValid)
            {
                return View(binding.ConvertTo<UpdatePersonViewModel>());
            }
            var item = DataContext.People
             .Include(person => person.PersonDoors)
             .ThenInclude(personDoors => personDoors.Door)
             .Where(p => p.Id == binding.Id)
             .Select(p => p)
             .FirstOrDefault();
            binding.ConvertTo<Person>(item);

            BindMetadataFields(item, binding.Mode);
            SynchronisePersonDoor(binding, item);

            DataContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
