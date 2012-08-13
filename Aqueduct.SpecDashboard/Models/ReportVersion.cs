using System;

namespace Aqueduct.SpecDashboard.Models
{
    public class ReportVersion
    {
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }

        public ReportTestStatistics Statistics { get; set; }

        public static ReportVersion Empty
        {
            get { return new ReportVersion { Date = DateTime.MinValue, Version = "0.0.0.0", Statistics = new ReportTestStatistics(), Path = string.Empty}; }
        }
    }
}
