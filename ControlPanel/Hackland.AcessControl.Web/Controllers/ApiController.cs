using Hackland.AccessControl.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        [HttpGet("test")]
        public ActionResult<bool> Test()
        {
            return Json(true);
        }

    }
}
