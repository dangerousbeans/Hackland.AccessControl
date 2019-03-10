using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Extensions;
using Hackland.AccessControl.Web.Models;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        protected DataContext DataContext { get; set; }
        private UserManager<User> UserManager;
        private string DefaultHiddenAdministratorEmailAddress = "agrath@hackland.nz";

        public UserController(UserManager<User> userManager, DataContext dataContext)
        {
            this.DataContext = dataContext;
            this.UserManager = userManager;
        }

        public IActionResult Index()
        {
            var users = (from d in DataContext.Users where d.UserName != DefaultHiddenAdministratorEmailAddress orderby d.UserName select d);

            var model = new UserListViewModel
            {
                Items = users.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //Pattern for these view models came from https://stackoverflow.com/questions/17107334/same-view-for-both-create-and-edit-in-mvc4/25374200
            var model = new CreateUserViewModel();
            model.Mode = CreateUpdateModeEnum.Create;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserBindingModel binding)
        {
            if (!ModelState.IsValid)
            {
                AddError("Error", ModelState, state =>  state.Children != null && state.Children.Any() /*checks for IValidatableObject (because those have children)*/ );
                return View(binding.ConvertTo<CreateUserViewModel>());
            }
            var item = binding.ConvertTo<User>();
            var user = new User
            {
                UserName = binding.EmailAddress,
                Email = binding.EmailAddress,
                FirstName = binding.FirstName,
                LastName = binding.LastName,
                PhoneNumber = binding.Phone
            };

            var result = await UserManager.CreateAsync(user, binding.Password);

            if (result.Errors.Any())
            {
                AddError("Error", string.Join("\r\n", result.Errors.Select(e => e.Description)));
                return View(binding.ConvertTo<CreateUserViewModel>());
            }

            DataContext.SaveChanges();
            AddSuccess("Success", $"Created user {binding.EmailAddress} successfully");
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = DataContext.Users
            .Where(p => p.Id == id)
            .Select(p => p)
            .FirstOrDefault();

            if (DataContext.Users.Where(u => u.UserName != DefaultHiddenAdministratorEmailAddress).Count() == 1)
            {
                AddError("Error", "Cannot delete the last user (because then noone would have access)");
                return RedirectToAction("Index");
            }

            DataContext.UserRoles.RemoveRange(DataContext.UserRoles.Where(ur => ur.UserId == id));
            DataContext.UserLogins.RemoveRange(DataContext.UserLogins.Where(ur => ur.UserId == id));
            DataContext.UserClaims.RemoveRange(DataContext.UserClaims.Where(ur => ur.UserId == id));
            DataContext.UserTokens.RemoveRange(DataContext.UserTokens.Where(ur => ur.UserId == id));
            DataContext.Users.Remove(item);

            var DefaultCreatedByUserId = DataContext.Users.Where(u => u.UserName == DefaultHiddenAdministratorEmailAddress).Select(u => u.Id).First();
            await DataContext.Doors.Where(d => d.CreatedByUserId == item.Id).ForEachAsync(d => d.CreatedByUserId = DefaultCreatedByUserId);
            await DataContext.Doors.Where(d => d.UpdatedByUserId == item.Id).ForEachAsync(d => d.UpdatedByUserId = DefaultCreatedByUserId);
            await DataContext.People.Where(d => d.CreatedByUserId == item.Id).ForEachAsync(d => d.CreatedByUserId = DefaultCreatedByUserId);
            await DataContext.People.Where(d => d.UpdatedByUserId == item.Id).ForEachAsync(d => d.UpdatedByUserId = DefaultCreatedByUserId);
            await DataContext.PeopleDoors.Where(d => d.CreatedByUserId == item.Id).ForEachAsync(d => d.CreatedByUserId = DefaultCreatedByUserId);
            await DataContext.PeopleDoors.Where(d => d.UpdatedByUserId == item.Id).ForEachAsync(d => d.UpdatedByUserId = DefaultCreatedByUserId);

            DataContext.SaveChanges();

            AddSuccess("Success", "Deleted User {0}", item.UserName);
            return RedirectToAction("Index");

        }

    }
}
