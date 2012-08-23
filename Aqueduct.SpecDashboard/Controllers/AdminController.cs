using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Aqueduct.SpecDashboard.Data;
using Aqueduct.SpecDashboard.Models;

namespace Aqueduct.SpecDashboard.Controllers
{
    public class AdminController : Controller
    {

        private readonly ReportRepository _repository;
        /// <summary>
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        public AdminController(ReportRepository repository)
        {
            _repository = repository;            
        }
        //
        // GET: /Admin/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            MembershipUser passwordChangedUser = null;
            var model = new AdminModel();
            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                var roles = Roles.GetRolesForUser(user.UserName);

                model.UserModels.Add(new UserModel
                {
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            var reports = _repository.GetReports();
            foreach (var report in reports)
            {
                var latestStats = report.Latest.Statistics;
                
                if (latestStats.IsEmpty == false)
                {                   
                    model.ReportModel.Add(new ReportViewModel
                    {
                        Name = report.Id
                    });                   
                }
            }
            
            if (passwordChangedUser != null)
            {
                model.NewPassword = passwordChangedUser.GetPassword();
            }
            return View(model);
        }

        // POST: /Admin/
        [HttpPost]
        public ActionResult Index(AdminModel model)
        {
          
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.RegisterModel.UserName, model.RegisterModel.Password, model.RegisterModel.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.RegisterModel.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        [HttpPost]
        public ActionResult ChangeParameters(string username, List<string> report)
        {
            var rolesToAdd = report ?? new List<string>();

            var currentRolesList = Roles.GetRolesForUser(username).Select(x => x.ToLower());
            foreach (var reportChecked in rolesToAdd)
            {
                
                if (currentRolesList.Contains(reportChecked.ToLower()) == false)
                {                 
                    if (Roles.RoleExists(reportChecked) == false)
                    {
                        Roles.CreateRole(reportChecked);
                    }
                    Roles.AddUserToRole(username, reportChecked);                  
                }                
            }

            var reports = _repository.GetReports();
            foreach (var reportsOriginal in reports)
            {
                if (rolesToAdd.Contains(reportsOriginal.Id) == false)
                {
                    if (currentRolesList.Contains(reportsOriginal.Id))
                    {
                        Roles.RemoveUserFromRole(username, reportsOriginal.Id);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteUser(string username)
        {
            if(username != "admin")
            {
                foreach (MembershipUser user in Membership.GetAllUsers())
                {
                    if (username == user.UserName)
                    {
                        Membership.DeleteUser(user.UserName);
                    }                   
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(string username)
        {
            var user = Membership.GetUser(username);
            if (user != null)
            {
                var password = user.ResetPassword();
                ViewBag.Message = string.Format("Password changed to: <b>{0}</b>", password);
            }
            else
            {
                ViewBag.Message = string.Format("Couldn't find user: {0}", username);
            }
            return View();
        }   
}}
