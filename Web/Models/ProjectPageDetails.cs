﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ProjectPageDetails
    {
        public ProjectModel Project { get; set; }
        public List<ActivityModel> Activities { get; set; }
        public List<EmployeeModel> AllEmployees { get; set; }
    }
}
