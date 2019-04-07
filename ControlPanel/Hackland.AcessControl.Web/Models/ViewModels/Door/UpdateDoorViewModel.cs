using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class UpdateDoorViewModel : UpdateDoorBindingModel, IDoorViewModel
    {
        public string MacAddress { get; set; }
    }
}
