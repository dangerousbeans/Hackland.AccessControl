using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public interface IUserViewModel
    {
        [Display(Name = "Email address")]
        [Required]
        string EmailAddress { get; set; }

        [Display(Name = "First name")]
        [Required]
        string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required]
        string LastName { get; set; }

        [Display(Name = "Phone")]
        [Required]
        [Phone]
        string Phone { get; set; }

        CreateUpdateModeEnum Mode { get; set; }
    }
}
