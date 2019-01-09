using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class AssociatePersonRequestViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public List<SelectListItem> AvailableTokens { get; set; }
    }
}
