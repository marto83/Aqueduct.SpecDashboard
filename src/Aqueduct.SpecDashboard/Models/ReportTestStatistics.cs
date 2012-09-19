using System;
namespace Aqueduct.SpecDashboard.Models
{
    public class ReportTestStatistics
    {
        private bool _empty = false;
        public bool IsEmpty
        {
            get { return _empty; }
        }

        public static ReportTestStatistics Empty
        {
            get
            {
                var stats = new ReportTestStatistics();
                stats._empty = true;
                return stats;
            }
        }

        public int Success { get; set; }
        public int Failures { get; set; }
        public int Inconclusive { get; set; }

        //    public double PercentageSuccessDifference { get; set; }
        public int NumberOfTests { get; set; }

        public double PercentageInconclusive
        {
            get
            {
                if (IsEmpty)
                    return 0;
                return (double)Inconclusive / NumberOfTests * 100.0;
            }
        }
        public double PercentageFailure
        {
            get
            {
                if (IsEmpty)
                    return 0;
                return (double)Failures / NumberOfTests * 100.0;
            }
        }
        public double PercentageSuccess
        {
            get
            {
                if (IsEmpty)
                    return 0;
                return (double)Success / NumberOfTests * 100.0;
            }
        }
    }
}