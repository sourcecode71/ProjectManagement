using Domain.Enums;

namespace Application.DTOs
{
    public class ProjectEmployDTO
    {
        public EmployeeType EmpType { get; set; }
        public string EmpId { get; set; }
        public double BudgetHours { get; set; }
        public string EmpName { get; set; }
        public string ProjectId { get; set; }
        public string WrkId { get; set; }
        public int EmpTypeId { get; set; }
    }


    public class WorkOrderEmployeeDTO
    {
        public EmployeeType EmpType { get; set; }
        public string Role { get; set; }
        public string Id { get; set; }
        public double Hour { get; set; }
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string WrkId { get; set; }
        public int EmpTypeId { get; set; }
    }
}
