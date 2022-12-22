using Application.Core.Projects;
using Application.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using PMG.Data.Repository.Projects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Web.ApiControllers
{
    public class ProjectController : BaseApiController
    {
        private readonly IProjectsRepository _project;

        public ProjectController(IProjectsRepository projects)
        {
            _project = projects;
        }



        [HttpPost("create-new-project")]
        public async Task<IActionResult> CreateNewProject([FromBody] ProjectDto dto)
        {
            bool Status = await _project.CreateProject(dto);
            return Ok(Status);
        }

        [HttpPost("update-project")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto dto)
        {
            bool Status = await _project.UpdateProject(dto);
            return Ok(Status);
        }

        [HttpGet("get-excel-report")]
        public FileResult ExportExcel(int projectType)
        {
            FileStream export = CreateExcel(projectType);
            return File(export, "application/octet-stream", "project_reports.xlsx");
        }

        [HttpGet("project-search")]
        public async Task<ActionResult> GetProjectSearchResult(string searchTag)
        {
            var project = await _project.GetProjectBySearch(searchTag);
            return Ok(project);
        }

        [HttpPost("project-budget/submit")]
        public async Task<ActionResult> SubmitProjectBUdget(ProjectApprovalDto project)
        {
            bool isApproved = await _project.SubmitBudget(project);
            return Ok(isApproved);
        }

        [HttpPost("budegt-approval/status")]
        public async Task<ActionResult> SaveWorkOrderApproval(ProjectApprovalDto dto)
        {


            string currentEmail = HttpContext.Session.GetString("current_user_email");
            if (!string.IsNullOrEmpty(currentEmail))
            {
                dto.SetUser = HttpContext.Session.GetString("current_user_id");
                bool isApproved = dto.Status == 3 ? await _project.BudgetChanges(dto) : await _project.ApprovalBudget(dto);
                return Ok(isApproved);
            }
            else
            {
                return Ok(false);
            }
        }


        [HttpGet("project-activities/budget-load")]
        public async Task<ActionResult> LoadProjectBudgetLoad(string PmName)
        {
            var ProjectBudgetAct = await _project.LoadWorkOrdeerBudgetAcitivies(PmName);
            return Ok(ProjectBudgetAct);
        }


        [HttpGet("load-active-projects")]
        public async Task<List<ProjectDto>> LoadActiveProjects()
        {
            try
            {
                var ProjectBudgetAct = await _project.GetAllActiveProjects();
                return ProjectBudgetAct;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("all-client")]
        public async Task<ActionResult> LoadAllClients()
        {
            var allClients = await _project.GetAllClient();
            return Ok(allClients);
        }

        [HttpGet("emp-wise/load-project")]
        public async Task<ActionResult<ProjectDto>> LoadAllProjectByEmp(string empId)
        {
            var allClients = await _project.GetAllProjects(empId);
            return Ok(allClients);
        }

        [HttpGet("project-status")]
        public ActionResult LoadProjectStatus()
        {
            Array values = Enum.GetValues(typeof(ProjectStatus));
            List<ProjectStatusDTO> items = new List<ProjectStatusDTO>(values.Length);

            foreach (var i in values)
            {
                items.Add(new ProjectStatusDTO
                {
                    StatusName = Enum.GetName(typeof(ProjectStatus), i),
                    Value = (int)i
                });
            }

            return Ok(items);
        }


        [HttpPut("project-activities/status")]
        public async Task<ActionResult> UpdateProjectStatus(ProjectCorrentStatusDTO dTO)
        {
            bool isStatusChange = await _project.UpdateProjectStatus(dTO);
            return Ok(isStatusChange);
        }



        private FileStream CreateExcel(int projectStatus)
        {
            var reportsFolder = @"C:\project-reports\\";

            List<ProjectDto> projects = Mediator.Send(new ListByProjectStatus.Query { ProjectStatus = projectStatus }).Result;

            string fileName = "Project report";
            string newPath = reportsFolder + fileName + ".xlsx";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage();
            ExcelWorkbook workbook = package.Workbook;
            ExcelWorksheet worksheet = workbook.Worksheets.Add("Project follow up");

            worksheet.Cells["A1:D1"].Merge = true;
            worksheet.Cells["A1:D1"].Value = "PROJECT FOLLOW UP - CONTROL SHEET";
            worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:D1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A1:D1"].Style.Font.Size = 12;
            worksheet.Cells["A1:D1"].Style.Font.Bold = true;

            worksheet.Cells["O4:Q4"].Merge = true;
            worksheet.Cells["O4:Q4"].Value = "COMENTARIOS";
            worksheet.Cells["O4:Q4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["O4:Q4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["O4:Q4"].Style.Font.Size = 10;

            worksheet.Cells["O5:Q5"].Merge = true;
            worksheet.Cells["O5:Q5"].Value = "STAFF COMMENTS";
            worksheet.Cells["O5:Q5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["O5:Q5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["O5:Q5"].Style.Font.Size = 10;

            worksheet.Cells["A6"].Value = "FOLIO";
            worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["A6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["A6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["A6"].Style.Font.Size = 10;
            worksheet.Cells["A6"].Style.Font.Bold = true;

            worksheet.Cells["B6"].Value = "PROYECTO";
            worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["B6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["B6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["B6"].Style.Font.Size = 10;
            worksheet.Cells["B6"].Style.Font.Bold = true;

            worksheet.Cells["C6"].Value = "CLIENTE";
            worksheet.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["C6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["C6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["C6"].Style.Font.Size = 10;
            worksheet.Cells["C6"].Style.Font.Bold = true;

            worksheet.Cells["D6"].Value = "INGENIER�A";
            worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["D6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["D6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["D6"].Style.Font.Size = 10;
            worksheet.Cells["D6"].Style.Font.Bold = true;

            worksheet.Cells["E6"].Value = "DIBUJO";
            worksheet.Cells["E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["E6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["E6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["E6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["E6"].Style.Font.Size = 10;
            worksheet.Cells["E6"].Style.Font.Bold = true;

            worksheet.Cells["F6"].Value = "VISTO BUENO";
            worksheet.Cells["F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["F6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["F6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["F6"].Style.Font.Size = 10;
            worksheet.Cells["F6"].Style.Font.Bold = true;

            worksheet.Cells["G6"].Value = "FECHA DE ENTREGA";
            worksheet.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["G6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["G6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["G6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["G6"].Style.Font.Size = 10;
            worksheet.Cells["G6"].Style.Font.Bold = true;

            worksheet.Cells["H6"].Value = "PROGRAMA";
            worksheet.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["H6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["H6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["H6"].Style.Font.Size = 10;
            worksheet.Cells["H6"].Style.Font.Bold = true;

            worksheet.Cells["I6"].Value = "SEMANA DE TRABAJO";
            worksheet.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["I6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["I6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["I6"].Style.Font.Size = 10;
            worksheet.Cells["I6"].Style.Font.Bold = true;

            worksheet.Cells["J6"].Value = "PRESUPUESTO";
            worksheet.Cells["J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["J6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["J6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["J6"].Style.Font.Size = 10;
            worksheet.Cells["J6"].Style.Font.Bold = true;

            worksheet.Cells["K6"].Value = "PAGADO";
            worksheet.Cells["K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["K6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["K6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["K6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["K6"].Style.Font.Size = 10;
            worksheet.Cells["K6"].Style.Font.Bold = true;

            worksheet.Cells["L6"].Value = "POR PAGAR";
            worksheet.Cells["L6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["L6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["L6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["L6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["L6"].Style.Font.Size = 10;
            worksheet.Cells["L6"].Style.Font.Bold = true;

            worksheet.Cells["M6"].Value = "FACTOR";
            worksheet.Cells["M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["M6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["M6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["M6"].Style.Font.Size = 10;
            worksheet.Cells["M6"].Style.Font.Bold = true;

            worksheet.Cells["N6"].Value = "ESTATUS";
            worksheet.Cells["N6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["N6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["N6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["N6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["N6"].Style.Font.Size = 10;
            worksheet.Cells["N6"].Style.Font.Bold = true;

            worksheet.Cells["O6"].Value = "INGENIER�A";
            worksheet.Cells["O6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["O6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["O6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["O6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["O6"].Style.Font.Size = 10;
            worksheet.Cells["O6"].Style.Font.Bold = true;

            worksheet.Cells["P6"].Value = "DIBUJO";
            worksheet.Cells["P6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["P6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["P6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["P6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["P6"].Style.Font.Size = 10;
            worksheet.Cells["P6"].Style.Font.Bold = true;

            worksheet.Cells["Q6"].Value = "ADMINISTRACION";
            worksheet.Cells["Q6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["Q6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells["Q6"].Style.Fill.SetBackground(eThemeSchemeColor.Accent1);
            worksheet.Cells["Q6"].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells["Q6"].Style.Font.Size = 10;
            worksheet.Cells["Q6"].Style.Font.Bold = true;

            worksheet.Cells["A6:Q20"].AutoFitColumns();

            int cellNumber = 7;

            foreach (var project in projects)
            {
                string cellId = "A" + cellNumber;
                string cellProject = "B" + cellNumber;
                string cellClient = "C" + cellNumber;
                string cellEng = "D" + cellNumber;
                string cellDraw = "E" + cellNumber;
                string cellApp = "F" + cellNumber;
                string cellDelivery = "G" + cellNumber;
                string cellSchedule = "H" + cellNumber;
                string cellProgress = "I" + cellNumber;
                string cellBudget = "J" + cellNumber;
                string cellPaid = "K" + cellNumber;
                string cellBalance = "L" + cellNumber;
                string cellFactor = "M" + cellNumber;
                string cellStatus = "N" + cellNumber;
                string cellComEng = "O" + cellNumber;
                string cellComDraw = "P" + cellNumber;
                string cellComAdmin = "Q" + cellNumber;

                worksheet.Cells[cellId].Value = project.SelfProjectId;
                worksheet.Cells[cellProject].Value = project.Name;
                worksheet.Cells[cellClient].Value = project.Client;
                worksheet.Cells[cellEng].Value = project.Engineering;
                worksheet.Cells[cellDraw].Value = project.Drawing;
                worksheet.Cells[cellApp].Value = project.Approval;
                worksheet.Cells[cellDelivery].Value = project.DeliveryDate.ToString("MM/dd/yyyy");
                worksheet.Cells[cellSchedule].Value = project.Schedule;
                worksheet.Cells[cellProgress].Value = project.Progress;
                worksheet.Cells[cellBudget].Value = CreateFormatForCurrency(project.Budget);
                worksheet.Cells[cellPaid].Value = CreateFormatForCurrency(project.Paid);
                worksheet.Cells[cellBalance].Value = CreateFormatForCurrency(project.Balance);
                worksheet.Cells[cellFactor].Value = CreateFormatForCurrency(project.Factor);
                worksheet.Cells[cellStatus].Value = project.Status;

                if (projectStatus == 0 || projectStatus == 3) //Delayed
                {
                    worksheet.Cells[cellComEng].Value = project.EmployeeDelayedComment;
                    worksheet.Cells[cellComDraw].Value = project.EmployeeDelayedComment;
                    worksheet.Cells[cellComAdmin].Value = project.AdminDelayedComment;
                }
                else if (projectStatus == 0 || projectStatus == 4) //Modified
                {
                    worksheet.Cells[cellComEng].Value = project.EmployeeModifiedComment;
                    worksheet.Cells[cellComDraw].Value = project.EmployeeModifiedComment;
                    worksheet.Cells[cellComAdmin].Value = project.AdminModifiedComment;
                }

                cellNumber++;
            }

            package.SaveAs(new FileInfo(newPath));
            package.Dispose();

            var fileStream = System.IO.File.OpenRead(newPath);

            return fileStream;
        }

        private string CreateFormatForCurrency(double amount)
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0:0,0.00}", amount);
        }
    }
}