using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackland.AccessControl.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hackland.AccessControl.Web.Controllers
{
    [AllowAnonymous]
    public class CreateUserBackdoorController : Controller
    {
        private UserManager<User> _userManager;

        public CreateUserBackdoorController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //disabled for security
            if (false)
            {
                string username = "user@fqdn.com";
                string password = "PASSWORDHERE";
                var user = new User { UserName = username };
                var result = await _userManager.CreateAsync(user, password);

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