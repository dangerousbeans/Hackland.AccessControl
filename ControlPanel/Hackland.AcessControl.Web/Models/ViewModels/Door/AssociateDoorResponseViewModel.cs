using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class AssociateDoorResponseViewModel
    {
        [Required]
        public int DoorReadId { get; set; }

        [Required(ErrorMessage = "Please choose a person")]
        [Display(Name = "Person")]
        public int? PersonId { get; set; }

        public int DoorId { get; set; }

        public List<SelectListItem> AvailablePeople { get; set; }
    }
}
