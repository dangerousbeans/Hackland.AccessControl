using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public class CreatePersonBindingModel : IPersonViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public List<SelectListItem> Doors { get; set; }

        public CreateUpdateModeEnum Mode { get; set; }


        public bool IsAccessTokenAssigned { get; set; }
    }
}
