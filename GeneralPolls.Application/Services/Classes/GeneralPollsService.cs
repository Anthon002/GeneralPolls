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
        public GeneralPollsService(IGeneralPollsRepository generalPollsRepository, UserManager<ApplicationUser> userManager)
        {
            _generalpollsrepository = generalPollsRepository;
            _userManager = userManager;
        }
        public Task<List<PollsViewModel>> ViewPolls()
        {
            return _generalpollsrepository.ViewPolls();
        }

        public Task<PollsViewModel> CreateNewPoll(PollsViewModel newPoll)
        {
            if (newPoll == null)
            {
                return (null);
            }
            return _generalpollsrepository.CreateNewPoll(newPoll);
        }

        public async Task<CandidateViewModel> AddCandidate(CandidateViewModel newCandidate)
        {
            var user = _userManager.Users.FirstOrDefault( x => x.Id == newCandidate.Id);
            if (user == null) { return (null); }
            CandidateDBModel candidate = new CandidateDBModel()
            {
                Id = newCandidate.Id,
                ElectionId = newCandidate.ElectionId,
                Email = user.Email,
                CandidateName = user.FirstName + " " + user.LastName,
                VoteCount = 0,
            };
            _generalpollsrepository.AddCandidate(candidate);

            return (newCandidate);
        }

        public Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId)
        {
            var registeredCandidates = _generalpollsrepository.ViewRegisteredCandidates(ElectionId);
            return (registeredCandidates);
        }

        public async Task<RegisteredVotersViewModel> RegisterVoter(string PollsId, string CurrentVoterID)
        {
            var newVoter = await _userManager.FindByIdAsync(CurrentVoterID);
            if (newVoter == null ){ return (null); }
            RegisteredVotersDBModel voterObj = new RegisteredVotersDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionId = PollsId,
                UserId = newVoter.Id,
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
        public async Task<CandidateViewModel> TransferVote(RegisteredVotersViewModel voter, string Id)
        {
            _generalpollsrepository.TransferVote(voter.Id, Id);
            return (null);
        }
    }
}
