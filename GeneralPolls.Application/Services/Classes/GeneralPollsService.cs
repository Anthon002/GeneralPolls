using GeneralPolls.Application.IRepositories;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using sib_api_v3_sdk.Api; 
using sib_api_v3_sdk.Client;  
using sib_api_v3_sdk.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using GeneralPolls.Core.Options;

namespace GeneralPolls.Application.Services.Classes
{
    public class GeneralPollsService: IGeneralPolls
    {
        private readonly IGeneralPollsRepository _generalpollsrepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleSeederService _roleseeder;
        private readonly IOptions<ApiKeysOptions> _options;

        public GeneralPollsService(IGeneralPollsRepository generalPollsRepository, UserManager<ApplicationUser> userManager, RoleSeederService roleseeder, IOptions<ApiKeysOptions> options)
        {
            _generalpollsrepository = generalPollsRepository;
            _userManager = userManager;
            _roleseeder = roleseeder;
            _options = options;
        }
        public Task<List<PollsViewModel>> ViewPolls()
        {
            return _generalpollsrepository.ViewPolls();
        }

        public async Task<string> CreateNewPoll(PollsViewModel newPoll,string UserId,string message )
        {
            if (newPoll == null)
            {
                return (null);
            }
            newPoll.Id = Guid.NewGuid().ToString();
            string response = await _generalpollsrepository.CreateNewPoll(newPoll, UserId);
            if (bool.Parse(response))
            {
                BackgroundJob.Schedule(()=> SendEndEmail(newPoll, UserId, message), DateTime.Parse( newPoll.EndDate.ToString()).ToUniversalTime());
                DateTime TransferTriggerTime = newPoll.EndDate.AddSeconds(30).ToUniversalTime();
                BackgroundJob.Schedule(()=> TransferToCompletedPoll(newPoll), TransferTriggerTime);
            }
            return response;
        }
        public async Task<string> TransferToCompletedPoll(PollsViewModel completedPoll)
        {
            var response =await _generalpollsrepository.TransferToCompletedPoll(completedPoll);
            return null;
        }
        public async Task<string> SendEndEmail(PollsViewModel completedPoll,string UserId, string message)
        {

            /**
             *First get list all registeredVoters using completedPoll.Id i.e all object with ElectionId == completedPoll.Id in repository
             *return list from repository to this method
             *add currentuser to the top of the list
             *using a loop repeat the brevo sending with a new recipient each iteration
             */
             try{
            List<string> registeredUsersEmail = await _generalpollsrepository.GetRegisteredVotersEmail(completedPoll.Id);
            foreach (var user in registeredUsersEmail)
            {
            Configuration.Default.ApiKey["api-key"] = _options.Value.BrevoApiKey;

            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "Chinedu Anulugwo";
            string SenderEmail = "chineduanulugwo@gmail.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            string ToEmail = user.ToLower();
            string ToName = user;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(ToEmail, ToName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
            string HtmlContent = null;
            string TextContent = $"{completedPoll.ElectionName} {message}{completedPoll.Id}";
            string Subject = "Election Winner";            
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, HtmlContent, TextContent, Subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine("Response: \n" + result.ToJson());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            }
            }
             catch(Exception ex)
             {
                //Outer try catch block for the entire method
                var errorMessage = ex.Message;
                var _InnerException = ex.InnerException; 
                Console.WriteLine(errorMessage);
                return null;
                
             }

            return null;
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
            var newVoter = await _userManager.FindByIdAsync(CurrentVoterID);
            
            if (newVoter == null ){ return (null); }
            RegisteredVotersDBModel voterObj = new RegisteredVotersDBModel()
            {
                Id = Guid.NewGuid().ToString(),
                ElectionId = PollsId,
                VoterId = CurrentVoterID,
                Vote = 1,
                VoterEmail = newVoter.Email
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
        public async Task<string> AssignCustomRoles(string userName, string pollId)
        {
            await _roleseeder.SeedCustomRoles(pollId);
           ApplicationUser user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, pollId);
            return null;
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
        public async Task<List<CompletedPollsViewModel>> GetCompletedPolls()
        {
            return await _generalpollsrepository.GetCompletedPolls();
        }
        public async Task<PollsViewModel> GetPoll(string Id)
        {
            return await _generalpollsrepository.GetPoll(Id);
        }

        public async Task<List<CandidateViewModel>> GetCandidateResultList(string Id)
        {
            return await _generalpollsrepository.GetCandidateResultList(Id);
        }

        public async Task<CompletedPollsViewModel> GetCompletedPoll(string Id)
        {
            return await _generalpollsrepository.GetCompletedPoll(Id);
        }
    }
}
