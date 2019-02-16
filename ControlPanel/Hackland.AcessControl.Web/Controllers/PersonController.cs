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
    public class PersonController : ControllerBase
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
            model.Doors = (from d in DataContext.Doors
                           orderby d.CreatedTimestamp descending
                           group d by d.MacAddress into g
                           let d = g.FirstOrDefault()
                           select new SelectListItem(d.Name == "Unknown" ? d.MacAddress : d.Name, d.Id.ToString(), model.Mode == CreateUpdateModeEnum.Create ? true : false, d.IsDeleted)).ToList();
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
            AddSuccess("Success", "Created person {0}", item.Name);
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

        [HttpGet]
        public IActionResult Update(int id)
        {
            var item = DataContext.People
                .Include(person => person.PersonDoors)
                .ThenInclude(personDoors => personDoors.Door)
                .Where(p => p.Id == id)
                .Select(p => p)
                .FirstOrDefault();

            if (item == null)
            {
                return RedirectToAction("Index");
            }

            var model = item.ConvertTo<UpdatePersonViewModel>();
            model.Mode = CreateUpdateModeEnum.Update;
            model.IsAccessTokenAssigned = !string.IsNullOrEmpty(item.TokenValue);
            BindAvailableDoors(model);

            //copy selected state into doors
            if (item.PersonDoors != null)
            {
                foreach (var selectedDoor in item.PersonDoors.Where(pd => !pd.IsDeleted).Select(pd => pd.DoorId))
                {
                    var selected = model.Doors.FirstOrDefault(d => string.Equals(d.Value, selectedDoor.ToString()));
                    if (selected != null)
                    {
                        selected.Selected = true;
                    }
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

            AddSuccess("Success", "Updated person {0}", item.Name);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = DataContext.People
            .Include(person => person.PersonDoors)
            .Where(p => p.Id == id)
            .Select(p => p)
            .FirstOrDefault();

            item.IsDeleted = true;
            BindMetadataFields(item, CreateUpdateModeEnum.Update);
            item.PersonDoors.ForEach(pd => BindMetadataFields(pd, CreateUpdateModeEnum.Update).IsDeleted = true);

            DataContext.SaveChanges();

            AddSuccess("Success", "Deleted person {0}", item.Name);
            return RedirectToAction("Index");

        }

        [HttpGet]
        [Route("person/assign-token/{id}")]
        public IActionResult AssignToken(int id)
        {
            var person = DataContext.People
                .Where(p => !p.IsDeleted)
                .FirstOrDefault(p => p.Id == id);

            if (person == null)
            {
                return RedirectToAction("Index");
            }

            AssociatePersonRequestViewModel model = GetAssociatePersonRequestModel(person);
            return View(model);

        }

        private AssociatePersonRequestViewModel GetAssociatePersonRequestModel(Person person)
        {
            return new AssociatePersonRequestViewModel
            {
                PersonId = person.Id,
                PersonName = person.Name,
                AvailableTokens = DataContext.DoorReads
                                .Where(dr => dr.PersonId == null)
                                .Include(dr => dr.Door)
                                .OrderByDescending(dr => dr.Timestamp)
                                .ToList()
                                .GroupBy(dr => new { dr.TokenValue, DoorName = dr.Door.Name })
                                .Select(dr => new SelectListItem
                                {
                                    Text = string.Format("{0} on {1} ({2:dd/MM/yyyy} {2:HH:mm})", dr.Key.TokenValue, dr.Key.DoorName, dr.FirstOrDefault().Timestamp),
                                    Value = dr.Key.TokenValue
                                })
                                .ToList()
            };
        }

        [HttpPost]
        [Route("person/assign-token")]
        public IActionResult AssignToken(AssociatePersonResponseViewModel model)
        {
            var person = DataContext.People.FirstOrDefault(p => p.Id == model.PersonId);

            if (!ModelState.IsValid)
            {
                AssociatePersonRequestViewModel requestModel = GetAssociatePersonRequestModel(person);
                return View(requestModel);
            }

            person.TokenValue = model.TokenValue;

            DataContext.SaveChanges();

            AddSuccess("Associated", "Assigned access token to {0}", person.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("person/clear-token/{id}")]
        public IActionResult ClearToken(int id)
        {
            var person = DataContext.People.FirstOrDefault(p => p.Id == id);

            person.TokenValue = null;

            DataContext.SaveChanges();

            AddSuccess("Associated", "Cleared access token for {0}", person.Name);

            return RedirectToAction("Index");
        }
    }
}
