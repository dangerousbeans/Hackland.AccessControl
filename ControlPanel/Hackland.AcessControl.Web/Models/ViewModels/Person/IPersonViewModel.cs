using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public interface IPersonViewModel
    {
        string Name { get; set; }
        [Display(Name = "Email address")]
        string EmailAddress { get; set; }
        [Display(Name = "Phone number")]
        string PhoneNumber { get; set; }
        List<SelectListItem> Doors { get; set; }

        CreateUpdateModeEnum Mode { get; set; }
    }
}
