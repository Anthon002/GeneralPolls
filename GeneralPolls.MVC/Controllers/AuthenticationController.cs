using Microsoft.AspNetCore.Mvc;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Application.Services.Interfaces;

namespace GeneralPolls.MVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _userauthentication;
        public AuthenticationController(IUserAuthenticationService userauthentication)
        {
            _userauthentication = userauthentication;
        }
        [HttpGet]
        public ActionResult<RegistrationViewModel> Register()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<RegistrationViewModel>> Register(RegistrationViewModel newUser)
        {
            if (!ModelState.IsValid) { return View(null); }
            await _userauthentication.Register(newUser);
                return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult<LoginViewModel> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<LoginViewModel>> Login( LoginViewModel user)
        {
            if (!ModelState.IsValid) { return View(null); }
                ViewData["nullUser"] = "You need to fill the fields to continue";
                ViewData["nullEmail"] = "You need to put in your Email";
                ViewData["nullPassword"] = "You need to put in your Password";
                if (user == null) { return View(ViewData["nullUser"]); }
                if (user.Username == null) { return View(ViewData["nullEmail"]); }
                if (user.Password == null) { return View(ViewData["nullPassword"]); }

                return View(await _userauthentication.Login(user));
        }
    }
}
