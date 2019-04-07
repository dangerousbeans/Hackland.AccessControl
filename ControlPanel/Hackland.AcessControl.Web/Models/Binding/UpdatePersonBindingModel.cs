using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models
{
    public class UpdatePersonBindingModel : CreatePersonBindingModel
    {
        [Required]
        public int? Id { get; set; }
    }
}
