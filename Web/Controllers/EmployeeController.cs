using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApiService _apiService;

        public EmployeeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            string currentEmail = HttpContext.Session.GetString("current_user_email");

            if (string.IsNullOrEmpty(currentEmail))
            {
                return RedirectToAction("Login", "Home");
            }

            EmployeePageDetails employeePageDetails = new EmployeePageDetails();
            EmployeeModel employee=_apiService.CallGetEmployees().Result.Where(x => x.Email == currentEmail).ToList().First();
            
            employeePageDetails.Projects = _apiService.CallGetProjects().Result.Where(x => (x.Status != "Archived" 
            && ((x.EmployeesNames!=null && x.EmployeesNames.Contains(employee.Name))
            || (x.Drawing != null && x.Drawing.Equals(employee.Name))
            || (x.Engineering != null && x.Engineering.Equals(employee.Name))
            || (x.Approval != null && x.Approval.Equals(employee.Name))
            ))).ToList();

            if (!string.IsNullOrEmpty(currentEmail))
            {
                employeePageDetails.Activities = _apiService.CallGetActivities(currentEmail).Result;
            }

            return View(employeePageDetails);
        }

        public List<ProjectModel> GetProjects()
        {
            return  _apiService.CallGetProjects().Result;
        }
    }
}