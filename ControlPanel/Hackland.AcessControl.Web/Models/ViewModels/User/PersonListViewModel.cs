using Hackland.AccessControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Models.ViewModels
{
    public class UserListViewModel
    {
        public IEnumerable<User> Items { get; set; }
    }
}
