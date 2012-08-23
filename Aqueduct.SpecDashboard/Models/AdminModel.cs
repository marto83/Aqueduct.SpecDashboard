using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Aqueduct.SpecDashboard.Models
{
    public class AdminModel
    {
        public RegisterModel RegisterModel { get; set; }

        public List<UserModel> UserModels { get; set; }

        public List<ReportViewModel> ReportModel { get; set; }

        public string PasswordResetUserName { get; set; }
        public string NewPassword { get; set; }

        public AdminModel()
        {
            RegisterModel = new RegisterModel();
            UserModels = new List<UserModel>();
            ReportModel = new List<ReportViewModel>();
        }
    }
}