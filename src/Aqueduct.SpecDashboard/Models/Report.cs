using System;
using System.Collections.Generic;
using System.Linq;

namespace Aqueduct.SpecDashboard.Models
{
    public class Report
    {
        public Report(string id)
        {
            Id = id;
            Versions = new List<ReportVersion>();
        }

        public string Id { get; private set; }

        public IList<ReportVersion> Versions { get; set; }

        public ReportVersion Latest
        {
            get { return Versions.OrderByDescending(x => x.Date).FirstOrDefault() ?? ReportVersion.Empty; }
        }

        public ReportVersion Previous
        {
            get
            {
                return Versions.OrderByDescending(x => x.Date).ElementAtOrDefault(1) ?? ReportVersion.Empty;
            }
        }
    }

    
}
