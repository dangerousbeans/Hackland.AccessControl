using Hackland.AccessControl.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class ResetUserPasswordViewModel : IPasswordViewModel, IValidatableObject
    {
        [Required]
        public Guid? Id { get; set; }

        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required]
        [Compare("Password", ErrorMessage = "Confirm password must match password")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = this.ValidatePassword(validationContext);
            foreach (var e in errors)
                yield return e;
        }
    }
}
