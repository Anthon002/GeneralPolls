using GeneralPolls.Application.IRepositories;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Model;
using GeneralPolls.Core.Models;
using GeneralPolls.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public GeneralPollsRepository(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<List<PollsViewModel>> ViewPolls()
        {
            var polls = _dbContext.PollsTable.Select(x => new PollsViewModel { ElectionName = x.ElectionName, UserId = x.UserId, Id = x.Id }).ToList();
            return (polls);
        }

        public async Task<string> CreateNewPoll(PollsViewModel newPoll, string UserId)
        {
            var createdPoll = new PollsDBModel()
            {
                Id = newPoll.Id,
                ElectionName = newPoll.ElectionName,
                UserId = UserId,
                DateCreated = DateTime.UtcNow,
                EndDate = DateTime.Parse( newPoll.EndDate.ToString()).ToUniversalTime(),
            };
            _dbContext.PollsTable.Add(createdPoll);
            var response = await _dbContext.SaveChangesAsync();
            if (response >= 1)
            {
                return "true";
            }
            return (response.ToString());
        }

        public async Task<string> AddCandidate(CandidateDBModel newCandidate)
        {
            int existingUsers = _dbContext.CandidateTable.Where(x => x.Email == newCandidate.Email).Where(x => x.ElectionId == newCandidate.ElectionId).Count();
          //  var UserExists = existingUsers.FirstOrDefault(x => x.ElectionId == newCandidate.ElectionId);

            try{
                if (existingUsers != 0)
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
            var alreadyRegistered = _dbContext.RegisteredVotersTable.ToList().Where(x => x.VoterId == newVoter.VoterId & x.ElectionId == newVoter.ElectionId); // This cross references the new voter's UserId and Election to know if this user has previously registerd for this same election/poll
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
               Console.WriteLine(ex.InnerException);
            }
            return (null);
        }

        public async Task<RegisteredVotersViewModel> GetRegisteredVoter(string ElectionId_, string Id_)
        {
            var voter = new RegisteredVotersViewModel();
            ApplicationUser user = await _userManager.FindByIdAsync(Id_);
            string _voterEmail = user.Email;
            var voterDB = _dbContext.RegisteredVotersTable.FirstOrDefault(x => x.VoterId == Id_ && x.ElectionId == ElectionId_);

            if (voterDB == null)
            {
                voter = new RegisteredVotersViewModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    ElectionId = ElectionId_,
                    Vote = -1,
                    UserId =Id_,
                    VoterEmail = _voterEmail
                };
            }
            else
            {

                voter = new RegisteredVotersViewModel()
                {
                    Id = voterDB.Id,
                    ElectionId = voterDB.ElectionId,
                    UserId = voterDB.VoterId,
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

        public async Task<string> DeleteCandidate(string CandidateId)
        {
            if (CandidateId == null){return ("Null Candidate Id");}
            var candidate = await _dbContext.CandidateTable.FirstOrDefaultAsync(x => x.Id == CandidateId);
            if (candidate == null){return ("Null Candidate");}
            var response = _dbContext.CandidateTable.Remove(candidate);
            _dbContext.SaveChanges();
            return ("Candidate Removed Successfully");
        }
        public async Task<bool> isPollForUser(string UserId, string PollId)
        {
            var poll = await _dbContext.PollsTable.FirstOrDefaultAsync(x => x.Id == PollId);
            if (poll == null){ return false; }
            if (poll.UserId != UserId){return false;}
            return true;
        }

        public async Task<string> TransferToCompletedPoll(PollsViewModel completedPoll)
        {
            try {
           PollsDBModel poll = await _dbContext.PollsTable.FirstOrDefaultAsync(x => x.Id == completedPoll.Id);
           if (poll == null){ return null; }
           CompletedPolls completed_poll = new CompletedPolls ()
           {
            Id = poll.Id,
            ElectionName = poll.ElectionName,
            UserId = poll.UserId,
            DateCreated = poll.DateCreated,
            EndDate = poll.EndDate,
           };
           await _dbContext.CompletedPollsTable.AddAsync(completed_poll);
            _dbContext.PollsTable.Remove(poll);
            var response = await _dbContext.SaveChangesAsync();
            if (response >= 1)
            {
                return "true";
            }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            return null;
        }

        public async Task<List<string>> GetRegisteredVotersEmail(string PollsId)
        {
            List<string> email = _dbContext.RegisteredVotersTable.Where(x => x.ElectionId == PollsId).Select(x => x.VoterEmail).ToList();
            return email;
        }
        public async Task<List<CompletedPollsViewModel>> GetCompletedPolls()
        {
            List<CompletedPollsViewModel> completedPolls = await (
                from polls in _dbContext.CompletedPollsTable
                select new CompletedPollsViewModel{
                    Id = polls.Id,
                    ElectionName = polls.ElectionName,
                    UserId = _dbContext.Users.FirstOrDefault(x => x.Id == polls.UserId).UserName,
                    DateCreated = polls.DateCreated,
                    EndDate = polls.EndDate,
                }
            ).ToListAsync();
            return completedPolls;
        }
        public async Task<PollsViewModel> GetPoll(string Id)
        {
            PollsViewModel pollVM= await (
            from polls in _dbContext.PollsTable
            where polls.Id == Id
            select new PollsViewModel{
                Id = polls.Id,
                ElectionName = polls.ElectionName,
                UserId = polls.UserId,
                DateCreated = polls.DateCreated,
                EndDate = polls.EndDate
            }
            ).FirstOrDefaultAsync();
            if (pollVM == null){return null;}
            return pollVM;

        }
        public async Task<List<CandidateViewModel>> GetCandidateResultList(string CompletedPollId)
        {
            List<CandidateViewModel> candidateList = await (
                from candidate in _dbContext.CandidateTable
                orderby candidate.VoteCount descending
                where candidate.ElectionId == CompletedPollId
                select new CandidateViewModel{
                    Id = candidate.Id,
                    ElectionId = candidate.ElectionId,
                    VoteCount = candidate.VoteCount,
                    CandidateName = candidate.CandidateName,
                    Email = candidate.Email,
                    CandidatePicturePath = candidate.CandidatePicturePath
                }
            ).ToListAsync();
            return candidateList;
        }
        public async Task<CompletedPollsViewModel> GetCompletedPoll(string Id)
        {
            CompletedPollsViewModel completedPoll = await(
                from poll in _dbContext.CompletedPollsTable
                where poll.Id == Id
                select new CompletedPollsViewModel
                {
                    Id = poll.Id,
                    ElectionName = poll.ElectionName,
                    UserId = poll.UserId,
                    DateCreated = poll.DateCreated,
                    EndDate = poll.EndDate,
                }
            ).FirstAsync();
            return completedPoll;
        }
        
    }
}
