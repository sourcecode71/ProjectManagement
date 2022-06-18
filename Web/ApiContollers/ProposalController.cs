using Application.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMG.Data.Repository.Proposal;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ApiControllers;

namespace Web.ApiContollers
{
    public class ProposalController : BaseApiController
    {
        private readonly IOTProposal _proposal;

        public ProposalController(IOTProposal proposal)
        {
            _proposal = proposal;
        }

        [HttpPost("create-new-proposal")]
        public async Task<IActionResult> CreateNewProject([FromBody] ProposalDTO dto)
        {
            dto.SetUserId = HttpContext.Session.GetString("current_user_id");
            bool Status = await _proposal.SubmitProposal(dto);
            return Ok(Status);
        }

        [HttpGet("proposal")]
        public async Task<IActionResult> GetProposal(Guid pid)
        {
            var proposal = await _proposal.GetProposal(pid);
            return Ok(proposal);
        }

        [HttpGet("all-active-proposal")]
        public async Task<IActionResult> GetAllActiveProposals()
        {
            var proposals = await _proposal.GetAllActiveProposal();
            return Ok(proposals);
        }

        [HttpPut("change-proposal-status")]
        public async Task<IActionResult> ChangeProposal(string pId, int appStatus)
        {
            bool status = await _proposal.UpdateApprovalStatus(pId, (EnumApprovalStatus)appStatus);
            return Ok(status);
        }
    }
}
