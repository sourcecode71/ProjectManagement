using Application.DTOs;
using Domain.Enums;
using Domain.Proposal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMG.Data.Repository.Proposal
{
    public interface IOTProposal
    {
        Task<bool> SubmitProposal(ProposalDTO dTO);
        Task<List<ProposalDTO>> GetAllActiveProposal();
        Task<Proposals> GetProposal(Guid Id);
        Task<bool> UpdateApprovalStatus(string proposalId, EnumApprovalStatus status);
    }
}
