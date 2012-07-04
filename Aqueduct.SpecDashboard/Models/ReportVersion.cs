using System;
using System.Collections.Generic;
using System.Linq;

namespace Aqueduct.SpecDashboard.Models
{
    public class ReportVersion
    {
        public DateTime Date { get; set; }
        public string Version { get; set; }

        public static ReportVersion Empty
        {
            get { return new ReportVersion { Date = DateTime.MinValue, Version = "0.0.0.0" }; }
        }
    }
}
