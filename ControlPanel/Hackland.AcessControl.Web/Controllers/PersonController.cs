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
    public class PersonController : Controller
    {
        protected DataContext DataContext { get; set; }

        public PersonController(DataContext dataContext)
        {
            this.DataContext = dataContext;
        }

        public IActionResult Index()
        {
            var people = (from d in DataContext.People where !d.IsDeleted select d);
            var model = new PersonListViewModel
            {
                Items = people.ToList()
            };

            return View(model);
        }

    }
}
