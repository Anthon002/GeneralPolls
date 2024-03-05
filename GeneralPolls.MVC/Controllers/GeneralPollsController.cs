using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GeneralPolls.MVC.Controllers
{
    public class GeneralPollsController : Controller
    {
        private readonly IGeneralPolls _generalPolls;
        private readonly IUserAuthenticationService _userAuthenticationService;
        public GeneralPollsController(IGeneralPolls generalPolls, IUserAuthenticationService userauthenticationService)
        {
            _generalPolls = generalPolls;
            _userAuthenticationService = userauthenticationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PollsViewModel>>> PollsPage()
        {
            var polls = await _generalPolls.ViewPolls();
            ViewData["Username"] = User.Identity.Name;
            return View(polls);
        }
        [HttpGet]
        public async Task<ActionResult<PollsViewModel>> CreatePoll()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<PollsViewModel>> CreatePoll(PollsViewModel newPoll)
        {
            ViewData["PollNull"] = "";
            ViewData["PollNameNull"] = "";

            if (newPoll == null)
            {
                ViewData["PollNull"] = "Make Sure Fill Out The Fields";
                return View(null);
            }
            if (newPoll.ElectionName == null)
            {
                ViewData["PollNameNull"] = "Your Poll Does Not Have A Name";
                return View(null);
            }
            Task<PollsViewModel> poll = _generalPolls.CreateNewPoll(newPoll);
            return RedirectToAction(nameof(PollsPage));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateViewModel>>> ViewPoll(string Id)
        {
            ViewData["PollsId"] = Id;
            var voter = await _userAuthenticationService.GetRegisteredVoter(Id);
            //ViewData["CurrentVoterID"] = voter;
            var registeredCandidate = _generalPolls.ViewRegisteredCandidates(Id).Result.ToList();
            if (voter.Vote == 0)
            {
                ViewData["ShowVotes"] = "True";
            }
            else
            {
                ViewData["ShowVotes"] = "False";
            }
            return View(registeredCandidate);
        }
        [HttpGet]
        public async Task<ActionResult<CandidateViewModel>> AddCandidates(string Id)
        {
            var a = _userAuthenticationService.GetUsers(Id).Result.ToList();
            ViewData["listofusers"] = a;
            ViewData["ElectionId"] = Id;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<CandidateViewModel>> StoreCandidate( CandidateViewModel newCandidate)
        {
            if (newCandidate == null) {return (null); }
            if (newCandidate.Id == null) { return (null); };
            await _generalPolls.AddCandidate(newCandidate);
            return RedirectToAction(nameof(ViewPoll));
        }


        [HttpGet]
        public async Task<ActionResult<string>> VoterRegisteration(string Id)
        {
            TempData["PollsId"] = Id;
            RegisteredVotersViewModel voter = await _userAuthenticationService.GetRegisteredVoter(Id); // currently logged on voter
            TempData["VoterID"] = voter.Id;
            return RedirectToAction("RegisterVoter", "GeneralPolls");
        }
        [HttpGet]
        public async Task<ActionResult<string>> RegisterVoter()
        {
            string PollsId = TempData["PollsId"].ToString();
            string CurrentVoterID = TempData["VoterID"].ToString();
            if (PollsId == null || CurrentVoterID == null) { return (null); }
            await _generalPolls.RegisterVoter(PollsId, CurrentVoterID);
            return (null);
        }

        [HttpGet]
        public async Task<ActionResult<RegisteredVotersViewModel>> Vote(string Id)
        {
            if(Id == null)
            {
                return (null);
            }
            CandidateViewModel candidate = await _generalPolls.GetCandidate(Id);
            ViewData["CandidateName"] = candidate.CandidateName;
            TempData["CandidateId"] = candidate.Id;
            RegisteredVotersViewModel voter = await _userAuthenticationService.GetRegisteredVoter(candidate.ElectionId); //Remove from authentication service and put in General Polls class
            return View(voter);
        }
        [HttpPost]
        public async Task<ActionResult<RegisteredVotersViewModel>> Vote(RegisteredVotersViewModel voter)
        {
            if (voter == null) { return (null); } // return message that reads
            string candidateId = TempData["CandidateId"].ToString();
            await _generalPolls.TransferVote(voter, candidateId);
            return RedirectToAction("PollsPage");
        }
    }
}
