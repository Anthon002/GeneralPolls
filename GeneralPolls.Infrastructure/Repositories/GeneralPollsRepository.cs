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
            var polls = _dbContext.PollsDB.Select(x => new PollsViewModel { ElectionName = x.ElectionName, CandidateCount = x.CandidateCount, Id = x.Id }).ToList();
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
            _dbContext.PollsDB.Add(createdPoll);
            _dbContext.SaveChanges();
            return (newPoll);
        }
    }
}
