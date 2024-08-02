using Microsoft.AspNetCore.Mvc;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Application.Services.Classes;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace GeneralPolls.MVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _userauthentication;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationController(IUserAuthenticationService userauthentication, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager,IHttpContextAccessor httpContextAccessor)
        {
            _userauthentication = userauthentication;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public ActionResult<RegistrationViewModel> Register()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<RegistrationViewModel>> Register(RegistrationViewModel newUser)
        {
            ViewData["nullUser"] = "";
            if (!ModelState.IsValid) { return View(null); }
            if (newUser.Password != newUser.ConfirmPassword) 
            {
                ViewData["nullUser"] = "Password and Confirm Password don't match";
            }
            var serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profile_pictures");
           // var serverFolder = "images/profile_pictures";
            var defaultfileName = "67c793c43fc94761baedb2884dcd1497_face.jpg";
            var completePath = Path.Combine(serverFolder, defaultfileName);
            var relativePath = Path.Combine("images/profile_pictures",defaultfileName);

            var newUser_ = new RegistrationViewModel(){FirstName = newUser.FirstName, LastName = newUser.LastName, Email = newUser.Email, Password = newUser.Password, ConfirmPassword=newUser.ConfirmPassword, File_Location = relativePath};
            string response = await _userauthentication.Register(newUser_);
            if (response == "userexists")
            {
                ViewData["nullUser"] = "This user is already registered";
                return View();
            }
                return RedirectToAction("ConfirmEmail","Authentication");
        }
        [HttpGet]
        public ActionResult<RegistrationViewModel> AdminRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<RegistrationViewModel>> AdminRegister(RegistrationViewModel adminUser)
        {
            if (adminUser == null)
            {
                return (null); // return flash error message
            };
            await _userauthentication.AdminRegister(adminUser);
            return RedirectToAction("PollsPage","GeneralPolls");
        }

        [HttpGet]
        public ActionResult<LoginViewModel> Login()
        {
            if(!User.Identity.IsAuthenticated)
            {
                ViewData["UserAuthenticated"] = "false";
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult<LoginViewModel>> Login(LoginViewModel user)
        {
            ViewData["nullUser"] = "";
            if (!ModelState.IsValid)
            {
                if (user == null)
                {
                    ViewData["nullUser"] = "You need to fill the fields to continue";
                    return View();
                }
                else if (user.Username == null)
                {
                    ViewData["nullUser"] = "You need to put in your Email";
                    return View();
                }
                else if (user.Password == null)
                {
                    ViewData["nullUser"] = "You need to put in your Password";
                    return View();
                }
            }
            var reponse = await _userauthentication.Login(user);
            ViewData["UserAuthenticated"] = "true";
            if (reponse == null)
            {
                ViewData["nullUser"] = "Incorrect Email or Password";
                return View();
            };
            return RedirectToAction("PollsPage", "GeneralPolls");
        }
        [HttpGet]
        public async Task<ActionResult> Settings()
        {
            string userId = _userManager.GetUserId(User);
            ViewData["userID"] = userId;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Settings(ApplicationUser updatedUser)
        {
            
            if (updatedUser == null) { return null; }
            var serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profile_pictures");
            var uniquefileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + updatedUser.ProfilePicture.FileName;
            var completePath = Path.Combine(serverFolder, uniquefileName);
            var completePathUpdated = completePath.Replace("\\", "/");

            if (!Directory.Exists(serverFolder))
            {
                Directory.CreateDirectory(serverFolder);
            }
            var relativePath = Path.Combine("images/profile_pictures", uniquefileName);
            updatedUser.ProfilePicture.CopyTo(new FileStream(completePath, FileMode.Create));
            var userUpdated = new ApplicationUser()
            {
                Id = updatedUser.Id,
                ProfilePicture = updatedUser.ProfilePicture,
                File_Location = relativePath,
            };
            await _userauthentication.ChangeProfilePicture(userUpdated);
            return RedirectToAction("PollsPage","GeneralPolls");
        }

        [HttpGet]
        public async Task<ActionResult<string>> Logout()
        {
            await _userauthentication.Logout();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult ConfirmEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ConfirmEmail(string response)
        {
            string confirmationToken = Guid.NewGuid().ToString().Replace("-", "");
            ApplicationUser user = await _userManager.GetUserAsync(User);
            var request = _httpContextAccessor.HttpContext.Request;
            if (user == null){return View("Null User");}
            var callBackUrl = Url.Action("ConfirmationComplete","Authentication",new {userId = user.Id, token = confirmationToken, protocol = HttpContext.Request.Scheme});
            // string emailContent = $"Please Confirm your email by clicking this link <a href='{callBackUrl}'>Click Here</a>";
            string emailContent = $"{request.Scheme}://{request.Host}{callBackUrl}";
            await _userauthentication.SendConfirmationLink(emailContent,user);
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ConfirmationComplete(string userID, string token)
        {
            if (userID == null || token == null)
            {
                return null;
            }
            ApplicationUser user = await _userManager.GetUserAsync(User);
            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("PollsPage","GeneralPolls");
            }
            else
            {
                Console.WriteLine(result);
                return null;
            }
            
            // await _userauthentication.EmailConfirmationTrue();
            return RedirectToAction("PollsPage","GeneralPolls");
        }

        // [HttpPost]
        // public async Task<ActionResult<ApplicationUser>> ShowProfilePicture()
        // {
        //     var userid = User.Identity.GetUserId().ToString();
        //     if (userid == null) { return null; }
        //     string user_image = _userauthentication.GetUserImage(userid);
        //     ViewData["Image_Path"] = "/images/profile_pictres/" + user_image;
        //     return (null);
        // }
    }
}
