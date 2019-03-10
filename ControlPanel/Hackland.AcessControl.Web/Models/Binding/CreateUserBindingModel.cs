using Hackland.AccessControl.Data;
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
    public class CreateUserBindingModel : IUserViewModel, IPasswordViewModel, IValidatableObject
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required]
        [Compare("Password", ErrorMessage = "Confirm password must match password")]
        public string ConfirmPassword { get; set; }

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var userManager = (UserManager<User>)validationContext.GetService(typeof(UserManager<User>));

            foreach(var validator in userManager.PasswordValidators)
            {
                var task = validator.ValidateAsync(userManager, null, this.Password);
                task.Wait();
                if(!task.Result.Succeeded)
                {
                    yield return new ValidationResult(string.Join("\r\n", task.Result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
