using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Domain;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Context;

namespace Application.Core.Projects
{
    public class List
    {
        public class Query : IRequest<List<ProjectDto>>
        {
        }

        public class Handler : IRequestHandler<Query, List<ProjectDto>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            private string GetStatusString(ProjectStatus status)
            {
                switch (status)
                {
                    case ProjectStatus.OnTime:
                        return "A tiempo";
                    case ProjectStatus.Delayed:
                        return "Demorado";
                    case ProjectStatus.Modified:
                        return "Modificado";
                    case ProjectStatus.Archived:
                        return "Archivado";
                    case ProjectStatus.Completed:
                        return "Terminado";
                    case ProjectStatus.Invoiced:
                        return "Facturado";
                    default:
                        return "A tiempo";
                }
            }

            public async Task<List<ProjectDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var projectsDto = new List<ProjectDto>();
                var projects = await _context.Projects.Include(x => x.ProjectEmployees).ToListAsync();
                foreach (var item in projects)
                {
                    var eng = item.ProjectEmployees.FirstOrDefault(x => x.ProjectId == item.Id && x.EmployeeType == EmployeeType.Engineering);
                    string engName = string.Empty;

                    if (eng != null)
                    {
                        var employee = _context.Employees.FirstOrDefault(x => x.Id == eng.EmployeeId);

                        if (employee != null)
                            engName = employee.Name;
                    }

                    var draw = item.ProjectEmployees.FirstOrDefault(x => x.ProjectId == item.Id && x.EmployeeType == EmployeeType.Drawing);
                    string drawName = string.Empty;

                    if (draw != null)
                    {
                        var employee = _context.Employees.FirstOrDefault(x => x.Id == draw.EmployeeId);

                        if (employee != null)
                            drawName = employee.Name;
                    }

                    var app = item.ProjectEmployees.FirstOrDefault(x => x.ProjectId == item.Id && x.EmployeeType == EmployeeType.Approval);
                    string appName = string.Empty;

                    if (app != null)
                    {
                        var employee = _context.Employees.FirstOrDefault(x => x.Id == app.EmployeeId);

                        if (employee != null)
                            appName = employee.Name;
                    }

                    var itemDto = new ProjectDto
                    {
                        Name = item.Name,
                        Description = item.Description,
                        SelfProjectId = item.SelfProjectId,
                        Balance = item.Balance,
                        Budget = item.Budget,
                        EStatus = item.EStatus,
                        Factor = item.Factor,
                        Paid = item.Paid,
                        Progress = Math.Abs(Math.Round((DateTime.Now - item.CreatedDate).TotalDays / 7, 2)),
                        Schedule = item.Schedule,
                        DeliveryDate = item.DeliveryDate,
                        Client = item.Client,
                        EstTime=item.EstTime,
                        Engineering = engName,
                        Drawing = drawName,
                        Approval = appName,
                        Status = GetStatusString(item.Status),
                        AdminDelayedComment = item.AdminDelayedComment,
                        AdminModifiedComment = item.AdminModifiedComment
                    };
                    projectsDto.Add(itemDto);
                }
                return projectsDto;
            }
        }
    }
}