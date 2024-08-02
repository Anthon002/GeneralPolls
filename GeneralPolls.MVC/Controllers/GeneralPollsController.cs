using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
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
        
        public bool IsConfirmed()
        {
            var emailConfirmationStatus = _userAuthenticationService.EmailConfirmationStatus();
            return emailConfirmationStatus;
        }


        [HttpGet]
        public async Task<ActionResult<List<PollsViewModel>>> PollsPage()
        {
            if(!User.Identity.IsAuthenticated)
            {
                ViewData["UserAuthenticated"] = "false";
                return RedirectToAction("Login","Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            ViewData["UserAuthenticated"] = "true";
            var user = User.Identity;

            var userid = User.Identity.GetUserId().ToString();
            if (userid == null) { return null; }
            string user_image = _userAuthenticationService.GetUserImage(userid);
            ViewData["Image_Path"] = user_image;
            TempData["Image_Path"] = user_image;

            var polls = await _generalPolls.ViewPolls();
            var user_ = _userAuthenticationService.GetUser();
            ViewData["Username"] = user_.FirstName;
            return View(polls);
        }

        [HttpGet]
        public async Task<ActionResult<PollsViewModel>> CreatePoll()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            ViewData["UserAuthenticated"] = "true";
            ViewData["Image_Path"] = TempData["Image_Path"] as String;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreatePoll(PollsViewModel newPoll)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

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
            string response = _generalPolls.CreateNewPoll(newPoll) ;

            if (response != null)
            {
                var userName = User.Identity.Name;
                await _generalPolls.AssignCustomRoles(userName,response);
            }
            return RedirectToAction(nameof(PollsPage));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidateViewModel>>> ViewPoll(string Id)
        {
            ViewData["UserAuthenticated"] = "";
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            ViewData["UserAuthenticated"] = "true";
            ViewData["PollsId"] = Id;
            ViewData["ShowAddCandidate"] = "False";
            ViewData["Unregistered"] = TempData["Unregistered"];
            ViewData["Image_Path"] = TempData["Image_Path"] as String;
            var voter = await _userAuthenticationService.GetRegisteredVoter(Id);
            //ViewData["CurrentVoterID"] = voter;
            if (User.IsInRole(Id))
            {
                ViewData["ShowAddCandidate"] = "True";
            }
            // var confirmationstatus = IsConfirmed();
            // if (confirmationstatus == false)
            // {
            //     return RedirectToAction("ConfirmEmail","Authentication");
            // }

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
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            ViewData["UserAuthenticated"] = "true";
            var a = _userAuthenticationService.GetUsers(Id).Result.ToList();
            ViewData["listofusers"] = a;
            ViewData["ElectionId"] = Id;
            ViewData["Image_Path"] = TempData["Image_Path"] as String;
            ViewData["addCandidate"] = TempData != null ? TempData["addCandidate"]: "";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<CandidateViewModel>> StoreCandidate( CandidateViewModel newCandidate)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            if (newCandidate == null) {return (null); }
            if (newCandidate.Id == null) { return (null); }
            var message = await _generalPolls.AddCandidate(newCandidate);
            if (message != "success")
            {
                TempData["addCandidate"] = message;
                return RedirectToAction("AddCandidate", new {id = newCandidate.ElectionId});
            }

            return RedirectToAction(nameof(ViewPoll), new { id = newCandidate.ElectionId }); //Error with getting the ViewPoll PollId after Adding a candidate;
        }

        [HttpGet]
        public async Task<ActionResult<string>> VoterRegisteration(string Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            ViewData["UserAuthenticated"] = "true";
            TempData["PollsId"] = Id;
            RegisteredVotersViewModel voter = await _userAuthenticationService.GetRegisteredVoter(Id);
            TempData["VoterID"] = voter.Id;
            ViewData["Image_Path"] = TempData["Image_Path"] as String;
            RegisterVoter(voter.UserId, voter.ElectionId);
            return RedirectToAction("ViewPoll", new { id = Id});
        }

        [HttpPost]
        public async Task<ActionResult<string>> RegisterVoter(string voter_id, string poll_id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            //string PollsId = TempData["PollsId"].ToString();
            //string CurrentVoterID = TempData["VoterID"].ToString();
            if (poll_id == null || voter_id == null) { return (null); }
            await _generalPolls.RegisterVoter(poll_id, voter_id);
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<RegisteredVotersViewModel>> Vote(string Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            if (Id == null)
            {
                return (null);
            }
            ViewData["UserAuthenticated"] = "true";
            CandidateViewModel candidate = await _generalPolls.GetCandidate(Id);
            ViewData["CandidateName"] = candidate.CandidateName;
            TempData["CandidateId"] = candidate.Id;
            ViewData["Image_Path"] = TempData["Image_Path"] as String;
            RegisteredVotersViewModel voter = await _userAuthenticationService.GetRegisteredVoter(candidate.ElectionId); //Remove from authentication service and put in General Polls class
            return View(voter);
        }

        [HttpPost]
        public async Task<ActionResult<RegisteredVotersViewModel>> Vote(RegisteredVotersViewModel voter)
        {
            TempData["Unregistered"] = "";
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var confirmationstatus = IsConfirmed();
            if (confirmationstatus == false)
            {
                return RedirectToAction("ConfirmEmail","Authentication");
            }

            if (voter == null) { return (null); } // return message that reads
            string candidateId = TempData["CandidateId"].ToString();
            string response =  _generalPolls.TransferVote(voter, candidateId);
            if (response == "Unregistered")
            {
                TempData["Unregistered"] = "You are not yet registered as a voter for this election";
            }
            return RedirectToAction("ViewPoll",new { id = voter.ElectionId});
        }
    }
}
