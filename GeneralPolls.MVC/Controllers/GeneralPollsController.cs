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
            var registeredVoters = _generalPolls.ViewRegisteredCandidates(Id).Result.ToList();
            return View(registeredVoters);
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
            if (newCandidate == null) {return View(null); }
            if (newCandidate.Id == null) { return (null); };
            await _generalPolls.AddCandidate(newCandidate);
            return RedirectToAction(nameof(ViewPoll));
        }
    }
}
