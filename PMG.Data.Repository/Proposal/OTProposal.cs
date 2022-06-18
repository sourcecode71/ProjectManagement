using Application.DTOs;
using Domain.Enums;
using Domain.Proposal;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PMG.Data.Repository.Proposal
{
    public class OTProposal : IOTProposal
    {
        private readonly DataContext _context;
        public OTProposal(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<List<ProposalDTO>> GetAllActiveProposal()
        {
            try
            {

            var proposals = await (from pp in _context.Proposals
                           join c in _context.Company on pp.CompanyId equals c.Id
                           where (pp.IsDeleted == false)
                           select new ProposalDTO { 
                            Id = pp.Id,
                            Comments=pp.Comments,
                            proposalNo=pp.ProposalNo,
                            ApprovalStatus=pp.ApprovalStatus,
                            CompanyId =pp.CompanyId,
                            CompanyName =c.ContactName,
                            EstimateBudget=pp.EstimateBudget,
                            ProposalName = pp.ProposalName,
                            SubmitDateStr = pp.SetDate.ToString("dd/MM/yyyy")

                           }).ToListAsync();
                proposals.Select(S => { S.ApprovalStatusStr = this.GetApprovalStatusName(S.ApprovalStatus); return S; }).ToList();
                return proposals;
            }
            catch(Exception ex)
            {
                throw ex;
            }



        }

        public async Task<bool> SubmitProposal(ProposalDTO dTO)
        {
            try
            {
                string proposalNo = this.GetProposalNo();
                Proposals proposals = new Proposals()
                {
                    Id = Guid.NewGuid(),
                    ApprovalStatus = EnumApprovalStatus.Submit,
                    ProposalNo=proposalNo,
                    CompanyId = dTO.CompanyId,
                    EstimateBudget = dTO.EstimateBudget,
                    Comments = dTO.Comments,
                    ProposalName = dTO.ProposalName,
                    IsDeleted = false,
                    SetUser = dTO.SetUserId,
                    SetDate = DateTime.Now
                };

                _context.Proposals.Add(proposals);
               var status= await _context.SaveChangesAsync();

                return status > 0;


            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<bool> UpdateApprovalStatus(string proposalId, EnumApprovalStatus status)
        {
            try
            {
                var proposal = _context.Proposals.FirstOrDefault(x => x.Id == new Guid(proposalId));
                if (proposal != null)
                {
                    proposal.ApprovalStatus = status;
                    int appStatus = await _context.SaveChangesAsync();
                    return appStatus >= 0;
                }
                else
                {
                    return false;
                }
              

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string GetProposalNo()
        {
            DateTime CurrentDate = DateTime.Now;
            var PmOTCount = _context.WorkOrder.Where(P => P.Year == CurrentDate.Year).Count() + 1;
            string ProposalNo = string.Format("{0}{1}", CurrentDate.Year, PmOTCount.ToString("00000"));
            return ProposalNo;
        }

        private string GetApprovalStatusName(EnumApprovalStatus value)
        {
            try
            {
                string appStatus = Enum.GetName(typeof(EnumApprovalStatus), value);
                return appStatus;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Proposals> GetProposal(Guid Id)
        {
            try
            {
                var proposal = await _context.Proposals.FirstOrDefaultAsync(p => p.Id == Id);
                return proposal;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
