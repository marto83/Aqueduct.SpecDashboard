using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aqueduct.SpecDashboard.Data;

namespace Aqueduct.SpecDashboard.Controllers
{
    public class ReportController : Controller
    {
        private readonly ReportRepository _repository;
        /// <summary>
        /// Initializes a new instance of the ReportController class.
        /// </summary>
        public ReportController(ReportRepository repository)
        {
            _repository = repository;            
        }

        public ActionResult Index(string id)
        {
            var report = _repository.FindReport(id);
            return View(report);
        }

        public ActionResult UploadReport(string id, string version, HttpPostedFileBase uploadFile)
        {
            _repository.AddReport(id, version, uploadFile.InputStream);
            return Content("Success");
        }
    }
}
