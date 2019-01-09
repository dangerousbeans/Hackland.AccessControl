using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class AssociatePersonResponseViewModel
    {
        public int PersonId { get; set; }

        [Required(ErrorMessage = "Please choose a token")]
        [Display(Name = "Token")]
        public string TokenValue { get; set; }

        public List<SelectListItem> AvailableTokens { get; set; }
    }
}
