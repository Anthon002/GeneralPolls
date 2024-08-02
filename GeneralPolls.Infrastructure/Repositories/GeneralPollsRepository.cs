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

        public string CreateNewPoll(PollsViewModel newPoll)
        {
            var createdPoll = new PollsDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionName = newPoll.ElectionName,
                CandidateCount = newPoll.CandidateCount,
            };
            _dbContext.PollsTable.Add(createdPoll);
            _dbContext.SaveChanges();
            return (createdPoll.Id);
        }

        public async Task<string> AddCandidate(CandidateDBModel newCandidate)
        {
            var existingUser = await _dbContext.CandidateTable.FirstOrDefaultAsync(x => x.Email == newCandidate.Email);
            try{
                if (existingUser == null)
                {
                    return ("This candidate already exists");
                }
            await _dbContext.CandidateTable.AddAsync(newCandidate);
            _dbContext.SaveChanges();
            
            }
            catch (Exception ex){
                Console.WriteLine(ex.InnerException);
            }
            return ("success");
        }
        public async Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId)
        {
            var registeredCandidates = _dbContext.CandidateTable.Select(x => new CandidateViewModel { ElectionId = x.ElectionId, CandidateName = x.CandidateName, Email = x.Email, Id = x.Id, VoteCount = x.VoteCount, CandidatePicturePath = x.CandidatePicturePath}).Where(x => x.ElectionId == ElectionId).ToList();
            return (registeredCandidates);
        }
        public async Task<RegisteredVotersViewModel> RegisterVoter(RegisteredVotersDBModel newVoter)
        {
            var alreadyRegistered = _dbContext.RegisteredVotersTable.ToList().Where(x => x.UserId == newVoter.UserId & x.ElectionId == newVoter.ElectionId); // This cross references the new voter's UserId and Election to know if this user has previously registerd for this same election/poll
            if (alreadyRegistered.Count() >= 1)
            {
                return (null);
            }
            try{
            _dbContext.RegisteredVotersTable.Add(newVoter);
            _dbContext.SaveChanges();
            }
            catch (Exception ex){
               Console.WriteLine(ex.Message);
            }
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
                    Id = Guid.NewGuid().ToString(),
                    ElectionId = ElectionId_,
                    Vote = -1,
                    UserId =Id_,
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
        public string TransferVote(string voterId,  string candidateId)
        {
            var voter = _dbContext.RegisteredVotersTable.FirstOrDefault(x => x.Id == voterId);
            var candidate = _dbContext.CandidateTable.FirstOrDefault(x => x.Id == candidateId);

            if(voter == null || candidate == null)
            {
                return ("Unregistered");
            }

            if (voter.Vote == 1)
            {
                voter.Vote = 0;
                candidate.VoteCount += 1;
                _dbContext.SaveChanges();
            }

            return (null);

        }
        public async Task<Boolean> UserExists(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) { return false;}
            return true;
        }

        public async Task<ApplicationUser> ChangeProfilePicture(ApplicationUser updatedUser)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == updatedUser.Id);
            user.ProfilePicture = updatedUser.ProfilePicture;
            user.File_Location = updatedUser.File_Location;
            _dbContext.SaveChanges();
            return user;

        }
        public string GetUserImage(string UserId)
        {
            if (UserId == null) { return (null); }
            ApplicationUser user = _dbContext.Users.FirstOrDefault(x => x.Id == UserId);
            if (user == null) { return null; }
            return user.File_Location;
        }
        public ApplicationUser GetUser(string userId)
        {
            ApplicationUser user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            return user;
        }

    }
}
