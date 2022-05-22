using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class BudgetDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public double OriginalBudget { get; set; }
        public List<BudgetHistoryDto> BHistory { get; set; }
    }

    public class BudgetHistoryDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public string Inv { get; set; }
        public double OriginalBudget { get; set; }
        public double Budget { get; set; }
        public string Comments { get; set; }
    }
}
