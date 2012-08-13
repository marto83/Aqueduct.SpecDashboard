using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aqueduct.SpecDashboard.Models;
using Aqueduct.SpecDashboard.Data;

namespace Aqueduct.SpecDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ReportRepository _repository;
        /// <summary>
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        public HomeController(ReportRepository repository)
        {
            _repository = repository;            
        }

        public ActionResult Index()
        {
            var reports = _repository.GetReports();

            //Create an object that can be easily used on the view to avoid logic inside the view
            List<ReportViewModel> viewReports = new List<ReportViewModel>();

            foreach (var report in reports)
            {
                //convert each report to a reportViewModel
                // report.
                // viewReports.Add(report);
                ReportViewModel viewReport = new ReportViewModel();
                var latestStats = report.Latest.Statistics;

                if (latestStats.IsEmpty == false)
                {
                    viewReport.HasDetailedStats = true;
                    viewReport.Date = report.Latest.Date;
                    viewReport.Name = report.Id;
                    viewReport.Success = latestStats.Success;

                    viewReport.NumberOfTests = latestStats.NumberOfTests;
                    viewReport.Inconclusive = latestStats.Inconclusive;
                    viewReport.Failures = latestStats.Failures;

                    viewReport.PercentageSuccess = (int)latestStats.PercentageSuccess;
                    viewReport.PercentageInconclusive = (int)latestStats.PercentageInconclusive;
                    viewReport.PercentageFailure = (int)latestStats.PercentageFailure;

                    GetPreviousVersionStats(report, viewReport, latestStats);
                }

                viewReports.Add(viewReport);
            }


            return View(viewReports);
        }
        private static void GetPreviousVersionStats(Report report, ReportViewModel viewReport, ReportTestStatistics latestStats)
        {
            if (report.Previous == null)
            {
                return;
            }

            var previousStats = report.Previous.Statistics;
            if (previousStats.IsEmpty)
            {
                return;
            }
                
            if (latestStats.PercentageSuccess > previousStats.PercentageSuccess)
            {
                viewReport.StateChange = ReportViewModel.ProjectStateChange.Good;
            }
            else if (latestStats.PercentageSuccess == previousStats.PercentageSuccess)
            {
                viewReport.StateChange = ReportViewModel.ProjectStateChange.Neutral;
            }
            else if (latestStats.PercentageSuccess < previousStats.PercentageSuccess)
            {
                viewReport.StateChange = ReportViewModel.ProjectStateChange.Bad;
            }
            viewReport.HasPreviousStats = true;
        }
    }


}
