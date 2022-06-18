using Domain.Enums;
using System;

namespace Application.DTOs
{
    public class ProposalDTO
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string proposalNo { get; set; }
        public string CompanyName { get; set; }
        public string ProposalName { get; set; }
        public double EstimateBudget { get; set; }
        public string Comments { get; set; }
        public string SetUserId { get; set; }
        public string SubmitDateStr { get; set; }
        public EnumApprovalStatus ApprovalStatus { get; set; }
        public string ApprovalStatusStr { get; set;}
    }
}
