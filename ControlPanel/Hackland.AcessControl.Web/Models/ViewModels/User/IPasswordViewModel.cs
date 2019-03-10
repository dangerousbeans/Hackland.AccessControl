using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public interface IPasswordViewModel
    {
        [Display(Name = "Password")]
        [Required]
        string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required]
        string ConfirmPassword { get; set; }
    }
}
