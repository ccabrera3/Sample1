using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.Data.Entities.Security
{
    public class ApplicationUser : IdentityUser
    {
        // have to go to references in chinook.Data -> System.Security -> using clause

        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }

    }
}
