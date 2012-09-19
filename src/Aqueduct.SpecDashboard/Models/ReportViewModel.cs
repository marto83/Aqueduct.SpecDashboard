using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aqueduct.SpecDashboard.Models
{
    public class ReportViewModel
    {
        public enum ProjectStateChange
        {
            Good,
            Bad,
            Neutral
        }

        public ProjectStateChange StateChange { get; set; }

        public int NumberOfTests { get; set; }

        public int Success { get; set; }

        public int Inconclusive { get; set; }

        public int Failures { get; set; }

        public int PercentageSuccess { get; set; }

        public int PercentageInconclusive { get; set; }

        public int PercentageFailure { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public bool HasDetailedStats { get; set; }
        public bool HasPreviousStats { get; set; }
        
        
    }
}