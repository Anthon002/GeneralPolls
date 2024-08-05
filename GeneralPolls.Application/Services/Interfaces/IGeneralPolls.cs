using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;

namespace GeneralPolls.Application.Services.Interfaces
{
    public interface IGeneralPolls
    {
        Task<List<PollsViewModel>> ViewPolls();
        string CreateNewPoll(PollsViewModel newPoll, string UserId);
        Task<string> AddCandidate(CandidateViewModel newCandidate);
        Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId);
        Task<RegisteredVotersViewModel> RegisterVoter(string PollsId, string CurrentVoterID);
        Task<CandidateViewModel> GetCandidate(string Id);
        string TransferVote(RegisteredVotersViewModel voter, string Id);
        Task AssignCustomRoles(string userName, string pollId);
        string DeleteCandidate(string CandidateId);
        bool IsPollForUser(string UserId, string PollId);
    }
}
