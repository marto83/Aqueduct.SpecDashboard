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
            return View(_repository.GetReports());
        }
    }
}
