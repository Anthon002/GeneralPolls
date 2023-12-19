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
        [HttpPost]
        public ActionResult<RegistrationViewModel> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<RegistrationViewModel>> Register(RegistrationViewModel newUser)
        {
          return View(_userauthentication.Register(newUser));
        }
    }
}
