using Application.DTOs;
using Domain;
using MediatR;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Core.Projects
{
    public class Create
    {
        public class Command : IRequest<Unit>
        {
            public Project Project { get; set; }
            public List<ProjectEmployDTO> ProejctEmp { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                int maxSelfId = 1;
                if (_context.Projects.Count() > 0)
                    maxSelfId = _context.Projects.Max(x => x.SelfProjectId);

                request.Project.SelfProjectId = maxSelfId + 1;
                request.Project.Id = Guid.NewGuid().ToString();
                // request.Project.Balance = request.Project.Budget;
                _context.Projects.Add(request.Project);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return SystemException("Error!");

                var project = _context.Projects.FirstOrDefault(x => x.Id == request.Project.Id);

                if (project == null) return SystemException("Error");

                List<ProjectEmployee> projectEmployees = new List<ProjectEmployee>();

                foreach (var employee in request.ProejctEmp)
                {
                    var employeeInDb = _context.Employees.FirstOrDefault(x => x.Email == employee.EmpId);

                    if (employeeInDb != null)
                    {
                        projectEmployees.Add(new ProjectEmployee
                        {
                            EmployeeId = employeeInDb.Id,
                            ProjectId = project.Id,
                            EmployeeType = employee.EmpType,
                            BudgetHours = employee.BudgetHours
                        });

                    }
                }

                _context.ProjectEmployees.AddRange(projectEmployees);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }

            private Unit SystemException(string v)
            {
                throw new NotImplementedException(v);
            }
        }
    }
}