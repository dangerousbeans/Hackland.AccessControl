using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackland.AccessControl.Shared;
using Hackland.AccessControl.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hackland.AccessControl.Web.Controllers
{
    [AllowAnonymous]
    public class CreateUserBackdoorController : Controller
    {
        private UserManager<User> UserManager;
        private readonly IOptions<ApplicationConfigurationModel> Config;

        public CreateUserBackdoorController(UserManager<User> userManager, IOptions<ApplicationConfigurationModel> config)
        {
            this.UserManager = userManager;
            this.Config = config;
        }
        public async Task<IActionResult> Index()
        {
            //disabled for security
            if (Config.Value.EnableCreateDefaultUserRoute)
            {
                string username = Config.Value.CreateDefaultUserUsername;
                string password = Config.Value.CreateDefaultUserPassword;
                var user = new User { UserName = username };
                var result = await UserManager.CreateAsync(user, password);

                if (result.Errors.Any())
                {
                    return Content(string.Join("\r\n", result.Errors.Select(e => e.Description)));
                }
                else
                {
                    return Content($"Created user {username} successfully with password {password}");
                }
            }
            else
            {
                return Content("The create user backdoor has been disabled for security reasons");
            }
        }
    }
}