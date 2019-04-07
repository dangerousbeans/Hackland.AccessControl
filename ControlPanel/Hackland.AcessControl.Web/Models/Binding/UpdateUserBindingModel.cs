using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public class UpdateUserBindingModel : IUserViewModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "First name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Phone")]
        [Required]
        [Phone]
        public string Phone { get; set; }

        public CreateUpdateModeEnum Mode { get; set; }

     
    }
}
