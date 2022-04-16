﻿using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMG.Data.Repository.Employee
{
    public interface IEmployeeRepository
    {
        Task<List<EmployeeDto>> GetAllActiveEmployee();
        Task<EmployeeDto> GetEmployeeById(string empId);
        Task<List<EpmProjectsDto>> GetEmpProjects(string empId);
        Task<List<EpmProjectsDto>> GetEmpWorkOrder(string empId);
        Task<List<HourslogDto>> EmployeHourLogSummery(string wrkId);
        Task<List<HourslogDto>> EmployeHourLogDetails(string wrkId);
        Task<List<HourslogDto>> GetEmpWisehourLogs(string empId);
        Task<List<HourslogDto>> EmployeHourLogSummeryAll();
        Task<List<HourslogDto>> GetEmpWiseWrkOThourLogs(string empId, string wrkId);
        Task<bool> UpdateEmaployeeAssignState(HourslogDto dto);
        Task<bool> SetupEmployeeForWrk(HourslogDto dto);
    }
}
