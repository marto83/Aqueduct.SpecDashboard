using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aqueduct.SpecDashboard
{
    public static class Settings
    {
        public static string ReportsFolder
        {
            get { return HttpContext.Current.Server.MapPath("~/ReportsData"); }
        }
    }
}