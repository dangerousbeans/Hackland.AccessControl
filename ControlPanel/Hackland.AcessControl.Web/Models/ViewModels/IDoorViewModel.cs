using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public interface IDoorViewModel
    {
        string Name { get; set; }

        CreateUpdateModeEnum Mode { get; set; }
    }
}
