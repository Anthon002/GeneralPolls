using GeneralPolls.Application.IRepositories;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using GeneralPolls.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Infrastructure.Repositories
{
    public class GeneralPollsRepository: IGeneralPollsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GeneralPollsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<PollsViewModel>> ViewPolls()
        {
            var polls = _dbContext.PollsTable.Select(x => new PollsViewModel { ElectionName = x.ElectionName, CandidateCount = x.CandidateCount, Id = x.Id }).ToList();
            return (polls);
        }

        public async Task<PollsViewModel> CreateNewPoll(PollsViewModel newPoll)
        {
            var createdPoll = new PollsDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionName = newPoll.ElectionName,
                CandidateCount = newPoll.CandidateCount,
            };
            _dbContext.PollsTable.Add(createdPoll);
            _dbContext.SaveChanges();
            return (newPoll);
        }

        public async Task<CandidateViewModel> AddCandidate(CandidateDBModel newCandidate)
        {
            _dbContext.CandidateTable.Add(newCandidate);
            _dbContext.SaveChanges();
            return (null);
        }
        public async Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId)
        {
            var registeredCandidates = _dbContext.CandidateTable.Select(x => new CandidateViewModel { ElectionId = x.ElectionId, CandidateName = x.CandidateName, Email = x.Email, Id = x.Id, VoteCount = x.VoteCount}).Where(x => x.ElectionId == ElectionId).ToList();
            return (registeredCandidates);
        }
        public async Task<RegisteredVotersViewModel> RegisterVoter(RegisteredVotersDBModel newVoter)
        {
            var alreadyRegistered = _dbContext.RegisteredVotersTable.ToList().Where(x => x.UserId == newVoter.UserId & x.ElectionId == newVoter.ElectionId); // This cross references the new voter's UserId and Election to know if this user has previously registerd for this same election/poll
            if (alreadyRegistered.Count() >= 1)
            {
                return (null);
            }
            _dbContext.RegisteredVotersTable.Add(newVoter);
            _dbContext.SaveChanges();
            return (null);
        }

        public async Task<RegisteredVotersViewModel> GetRegisteredVoter(string ElectionId_, string Id_)
        {
            var voter = new RegisteredVotersViewModel();
            var voterDB = _dbContext.RegisteredVotersTable.FirstOrDefault(x => x.UserId == Id_ && x.ElectionId == ElectionId_);

            if (voterDB == null)
            {
                voter = new RegisteredVotersViewModel()
                {
                    Id = Id_,
                    ElectionId = ElectionId_,
                    Vote = -1,
                    UserId = Guid.NewGuid().ToString(),
                };
            }
            else
            {

                voter = new RegisteredVotersViewModel()
                {
                    Id = voterDB.Id,
                    ElectionId = voterDB.ElectionId,
                    UserId = voterDB.UserId,
                    Vote = voterDB.Vote,
                };
            }


            return (voter);
        }
        public async Task<CandidateViewModel> GetCandidate(string Id)
        {
            var candidate = await _dbContext.CandidateTable.FirstOrDefaultAsync(x => x.Id == Id);
            if (candidate == null) { return (null); } //Return code that represents candiate not found
            CandidateViewModel candidateViewModel = new CandidateViewModel()
            {
                Id = candidate.Id,
                VoteCount = candidate.VoteCount,
                CandidateName = candidate.CandidateName,
                ElectionId = candidate.ElectionId,
                Email = candidate.Email,
            };
            return (candidateViewModel);
        }
        public async void TransferVote(string voterId,  string candidateId)
        {
            var voter =  _dbContext.RegisteredVotersTable.FirstOrDefault(x => x.Id == voterId);
            var candidate = _dbContext.CandidateTable.FirstOrDefault(x => x.Id == candidateId);

            if(voter == null || candidate == null)
            {
                return; // change the return type from void to return a code that displays a message about no voter or candidate value
            }
            //subtract voter's vote
            if (voter.Vote == 1)
            {
                voter.Vote = 0;
                candidate.VoteCount += 1;
                _dbContext.SaveChanges();
            }

            //test the voting 

        }
    }
}
