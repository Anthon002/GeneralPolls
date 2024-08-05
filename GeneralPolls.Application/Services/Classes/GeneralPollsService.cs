using GeneralPolls.Application.IRepositories;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Application.Services.Classes
{
    public class GeneralPollsService: IGeneralPolls
    {
        private readonly IGeneralPollsRepository _generalpollsrepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleSeederService _roleseeder;

        public GeneralPollsService(IGeneralPollsRepository generalPollsRepository, UserManager<ApplicationUser> userManager, RoleSeederService roleseeder)
        {
            _generalpollsrepository = generalPollsRepository;
            _userManager = userManager;
            _roleseeder = roleseeder;
        }
        public Task<List<PollsViewModel>> ViewPolls()
        {
            return _generalpollsrepository.ViewPolls();
        }

        public string CreateNewPoll(PollsViewModel newPoll,string UserId)
        {
            if (newPoll == null)
            {
                return (null);
            }
            return _generalpollsrepository.CreateNewPoll(newPoll, UserId);
        }

        public async Task<string> AddCandidate(CandidateViewModel newCandidate)
        {
            var user = _userManager.Users.FirstOrDefault( x => x.Id == newCandidate.Id);
            if (user == null) { return (null); }
            CandidateDBModel candidate = new CandidateDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionId = newCandidate.ElectionId,
                Email = user.Email,
                CandidateName = user.FirstName + " " + user.LastName,
                VoteCount = 0,
                CandidatePicturePath = user.File_Location,
            };
            string candidateMsg = await _generalpollsrepository.AddCandidate(candidate);

            return (candidateMsg);
        }

        public Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId)
        {
            var registeredCandidates = _generalpollsrepository.ViewRegisteredCandidates(ElectionId);
            return (registeredCandidates);
        }

        public async Task<RegisteredVotersViewModel> RegisterVoter(string PollsId, string CurrentVoterID)
        {
            var newVoter = _userManager.FindByIdAsync(CurrentVoterID);
            if (newVoter == null ){ return (null); }
            RegisteredVotersDBModel voterObj = new RegisteredVotersDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionId = PollsId,
                UserId = CurrentVoterID,
                Vote = 1
            };
          
            _generalpollsrepository.RegisterVoter(voterObj);
            return (null);
        }
        public async Task<CandidateViewModel> GetCandidate(string Id)
        {
            CandidateViewModel candidate = await _generalpollsrepository.GetCandidate(Id);
            return (candidate);
        }
        public string TransferVote(RegisteredVotersViewModel voter, string Id)
        {
            string response = _generalpollsrepository.TransferVote(voter.Id, Id);
            return (response);
        }
        public async Task AssignCustomRoles(string userName, string pollId)
        {
            await _roleseeder.SeedCustomRoles(pollId);
           ApplicationUser user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, pollId);
        }
        public string DeleteCandidate(string CandidateId)
        {
            Task<string> response = _generalpollsrepository.DeleteCandidate(CandidateId);
            string responseString = response.Result;
            return responseString;
        }
        public bool IsPollForUser(string UserId, string PollId)
        {
            bool response = _generalpollsrepository.isPollForUser(UserId, PollId).Result;
            return response;
        }

    }
}
