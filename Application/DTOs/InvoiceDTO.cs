﻿using System;

namespace Application.DTOs
{
    public class InvoiceDTO
    {
        public string Id { get; set; }
        public string WorkOrderId { get; set; }
        public string CompanyName { get; set; }
        public string WorkNo { get; set; }
        public string ProjectId { get; set; }
        public double PartialBill { get; set; }
        public double InvoiceBill { get; set; }
        public double Balance { get; set; }
        public double SpentHour { get; set; }
        public double BudgetHour { get; set; }
        public string InvoiceNumber { get; set; }
        public string OTName { get; set; }
        public string ProjectName { get; set; }
        public string WorkOrderName { get; set; }
        public string ClientName { get; set; }
        public double OriginalBudget { get; set; }
        public double ApprovedBudget { get; set; }
        public string ApprovedDateStr { get; set; }
        public string Remarks { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceDateStr { get; set; }
        public string SetUser { get; set; }
        public string DueDateStr { get; set; }
        public DateTime SetDate { get; set; }
    }
}
