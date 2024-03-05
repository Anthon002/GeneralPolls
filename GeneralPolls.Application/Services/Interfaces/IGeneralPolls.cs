using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Core.DTOs;

namespace GeneralPolls.Application.Services.Interfaces
{
    public interface IGeneralPolls
    {
        Task<List<PollsViewModel>> ViewPolls();
        Task<PollsViewModel> CreateNewPoll(PollsViewModel newPoll);
        Task<CandidateViewModel> AddCandidate(CandidateViewModel newCandidate);
        Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId);
        Task<RegisteredVotersViewModel> RegisterVoter(string PollsId, string CurrentVoterID);
        Task<CandidateViewModel> GetCandidate(string Id);
        Task<CandidateViewModel> TransferVote(RegisteredVotersViewModel voter, string Id);
    }
}
