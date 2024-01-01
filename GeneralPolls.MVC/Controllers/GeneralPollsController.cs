using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GeneralPolls.MVC.Controllers
{
    public class GeneralPollsController : Controller
    {
        private readonly IGeneralPolls _generalPolls;
        public GeneralPollsController(IGeneralPolls generalPolls)
        {
            _generalPolls = generalPolls;
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
            return View(newPoll);
        }
        [HttpGet]
        public ActionResult AddCandidates()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<CandidateViewModel>> AddCandidate( CandidateViewModel newCandidate)
        {
            if (newCandidate == null)
            {
                return View(null); };)
            await _generalPolls.AddCandidate(newCandidate); //Next task: Service and Repository for Adding a Candidate PS: Admin would be the one to fill out the form to register a candidate
            return View();
        }
    }
}
