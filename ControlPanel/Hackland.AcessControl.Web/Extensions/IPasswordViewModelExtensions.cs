using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class IPasswordViewModelExtensions
{

    public static IEnumerable<ValidationResult> ValidatePassword(this IPasswordViewModel model, ValidationContext validationContext)
    {
        var userManager = (UserManager<User>)validationContext.GetService(typeof(UserManager<User>));

        foreach (var validator in userManager.PasswordValidators)
        {
            var task = validator.ValidateAsync(userManager, null, model.Password);
            task.Wait();
            if (!task.Result.Succeeded)
            {
                yield return new ValidationResult(string.Join("\r\n", task.Result.Errors.Select(e => e.Description)));
            }
        }
    }

}