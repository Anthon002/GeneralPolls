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
    }
}
