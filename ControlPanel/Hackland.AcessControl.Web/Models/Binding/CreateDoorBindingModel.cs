using Hackland.AccessControl.Data;
using Hackland.AccessControl.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public class CreateDoorBindingModel : IDoorViewModel
    {
        [Required]
        public string Name { get; set; }

        public CreateUpdateModeEnum Mode { get; set; }
    }
}
