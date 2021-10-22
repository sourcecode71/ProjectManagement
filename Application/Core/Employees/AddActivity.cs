using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistance.Context;

namespace Application.Core.Employees
{
    public class AddActivity
    {
        public class Command : IRequest<Unit>
        {
            public string EmployeeEmail { get; set; }
            public string ProjectId { get; set; }
            public double Duration { get; set; }
            public string Comment { get; set; }
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
                var project = _context.Projects.FirstOrDefault(x => x.Id == request.ProjectId);
                
                var emp = _context.Employees.FirstOrDefault( x => x.Email == request.EmployeeEmail);

                var activity = new ProjectActivity
                {
                    Id = Guid.NewGuid().ToString(),
                    Project= project,
                    ProjectId = request.ProjectId,
                    Employee = emp,
                    EmployeeId = emp.Id,
                    Duration = request.Duration,
                    Comment = request.Comment,
                    DateTime = DateTime.Now
                };

                _context.ProjectActivities.Add(activity);
                
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return SystemException("Greska pri dodavanju aktivnosti u bazu!");
                
                project.Activities.Add(activity);

                return Unit.Value;
            }

            private Unit SystemException(string v)
            {
                throw new NotImplementedException();
            }
        }
    }
}