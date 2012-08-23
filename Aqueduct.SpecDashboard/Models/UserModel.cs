using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aqueduct.SpecDashboard.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; }

        public UserModel()
        {
            Roles = new List<string>();
        }
    }
}