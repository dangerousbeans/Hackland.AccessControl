using Hackland.AccessControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class PersonListViewModel
    {
        public IEnumerable<Person> Items { get; set; }
    }
}
