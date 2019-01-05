using Hackland.AccessControl.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hackland.AccessControl.Web.Controllers
{
    [Authorize]
    public class DoorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
