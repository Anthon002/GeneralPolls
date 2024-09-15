using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Http;

namespace GeneralPolls.Application.Services.Interfaces
{
    public interface IGeneralPolls
    {
        Task<List<PollsViewModel>> ViewPolls();
        Task<string> CreateNewPoll(PollsViewModel newPoll, string UserId, string message);
        Task<string> AddCandidate(CandidateViewModel newCandidate);
        Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId);
        Task<RegisteredVotersViewModel> RegisterVoter(string PollsId, string CurrentVoterID);
        Task<CandidateViewModel> GetCandidate(string Id);
        string TransferVote(RegisteredVotersViewModel voter, string Id);
        Task<string> AssignCustomRoles(string userName, string pollId);
        string DeleteCandidate(string CandidateId);
        bool IsPollForUser(string UserId, string PollId);
        Task<List<CompletedPollsViewModel>> GetCompletedPolls();
        Task<PollsViewModel> GetPoll(string Id);
        Task<List<CandidateViewModel>> GetCandidateResultList(string Id);
        Task<CompletedPollsViewModel> GetCompletedPoll(string Id);
    }
}
