namespace Aqueduct.SpecDashboard.Models
{
    public class ReportTestStatistics
    {
        public int Success { get; set; }
        public int Failures { get; set; }
        public int Inconclusive { get; set; }

        //    public double PercentageSuccessDifference { get; set; }
        public int NumberOfTests { get; set; }

        public double PercentageInconclusive { get { return (double)Inconclusive / NumberOfTests * 100.0; } }
        public double PercentageFailure { get { return (double)Failures / NumberOfTests * 100.0; } }
        public double PercentageSuccess { get { return (double)Success / NumberOfTests * 100.0; } }
    }
}