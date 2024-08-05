using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Application.IRepositories
{
    public interface IGeneralPollsRepository
    {
        Task<List<PollsViewModel>> ViewPolls();
        string CreateNewPoll( PollsViewModel newPoll, string UserId);
        Task<string> AddCandidate( CandidateDBModel newCandidate );
        Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId);
        Task<RegisteredVotersViewModel> RegisterVoter(RegisteredVotersDBModel newVoter);
        Task<RegisteredVotersViewModel> GetRegisteredVoter(string ElectionId,string Id);
        Task<CandidateViewModel> GetCandidate(string Id);
        string TransferVote(string voterId, string candidateId);
        Task<Boolean> UserExists(string email);
        Task<ApplicationUser> ChangeProfilePicture(ApplicationUser udpatedUser);
        string GetUserImage(string UserId);
        ApplicationUser GetUser(string userId);
        // Task<bool> EmailConfirmationTrue();
        Task<string> DeleteCandidate(string CandidateId);
        Task<bool> isPollForUser(string UserId, string PollId);
        
    }
}
