using Domain.Common;
using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Proposal
{
    public class Proposals :BasedModel
    {
        [Key]
        public Guid Id { get; set; } 
        [Required, MaxLength(20)]  
        public string ProposalNo { set; get; }
        [Required,MaxLength(50)]
        public string ProposalName { get; set; }
        public double EstimateBudget { get; set; }
        public Guid CompanyId { get; set; }
        public virtual Company Companies { get; set; }
        public EnumApprovalStatus ApprovalStatus { get; set; }
        [MaxLength(300)]
        public string Comments { get; set; }

    }
}
