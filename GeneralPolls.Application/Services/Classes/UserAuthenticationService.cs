using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Application.IRepositories;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using sib_api_v3_sdk.Api;   // Brevo Email for sending mails using apikeys
using sib_api_v3_sdk.Client;    // Brevo Email for sending mails using apikeys
using sib_api_v3_sdk.Model; // Brevo Email for sending mails using apikeys

namespace GeneralPolls.Application.Services.Classes
{
    public class UserAuthenticationService: IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleSeederService _roleSeeder;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGeneralPollsRepository _generalPollsRepository;

        public UserAuthenticationService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contextAccessor, IGeneralPollsRepository generalPollsRepository, RoleSeederService roleSeeder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextAccessor = contextAccessor;
            _generalPollsRepository = generalPollsRepository;
            _roleSeeder = roleSeeder;
            _roleManager = roleManager;
        }

        public async Task<string> Register(RegistrationViewModel newUser)
        {
            if (newUser == null)
            {
                return (null);
            }
            bool userExists = await _generalPollsRepository.UserExists(newUser.Email);
            if (userExists) { return ("userexists"); }
            var user = new ApplicationUser() { Id = Guid.NewGuid().ToString(), UserName = newUser.Email, Email = newUser.Email, NormalizedEmail = newUser.Email.ToUpper(), FirstName = newUser.FirstName,LastName = newUser.LastName, File_Location = newUser.File_Location};
            var response = await _userManager.CreateAsync(user, newUser.Password);
            if (response.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return (null);

        }
        public async Task<RegistrationViewModel> AdminRegister(RegistrationViewModel adminUser)
        {
            if(adminUser == null)
            {
                return (null); // return a flash error message
            }
            await _roleSeeder.SeedRoles();
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = adminUser.Email,
                Email = adminUser.Email,
                NormalizedEmail = adminUser.Email.ToUpper(),
                FirstName = adminUser.FirstName,
                LastName = adminUser.LastName
            };

            var response = await _userManager.CreateAsync(user, adminUser.Password);

            if (response.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin"); ;
                await _signInManager.SignInAsync(user, isPersistent: true);
            }
            return (null);
        }
        public async Task<LoginViewModel> Login(LoginViewModel user)
        {
            var registeredUser = await _userManager.FindByNameAsync(user.Username);
            if (registeredUser == null ){ return (null);}
            var response = await _userManager.CheckPasswordAsync(registeredUser, user.Password);
            if (response)
            {
                await _signInManager.SignInAsync(registeredUser, isPersistent: false);
                return (user);
            }
            return (null);
        }
        public async Task<string> Logout()
        {
            await _signInManager.SignOutAsync();
            return (null);
        }
        public async Task<IEnumerable<CandidateViewModel>> GetUsers(string ElectionId)
        {
            var listofusers = _userManager.Users.Select(x => new CandidateViewModel() { CandidateName = x.FirstName, Email = x.Email, Id = x.Id, ElectionId = ElectionId, VoteCount = 0}).ToList();
            return (listofusers);
        }
        public async Task<RegisteredVotersViewModel> GetRegisteredVoter(string ElectionId)
        {
            var userId = _userManager.GetUserId(_contextAccessor.HttpContext.User);
            if (userId == null) { return null; }
            RegisteredVotersViewModel user = await _generalPollsRepository.GetRegisteredVoter(ElectionId,userId);
            return (user);
        }
        public async Task<ApplicationUser> ChangeProfilePicture(ApplicationUser updatedUser)
        {
            var user = await _generalPollsRepository.ChangeProfilePicture(updatedUser);
            return (user);
        }
        public string GetUserImage(string UserId)
        {
            return _generalPollsRepository.GetUserImage(UserId);
        }
        public async Task<bool> SendConfirmationLink(string emailContent, ApplicationUser user)
        {
            Configuration.Default.ApiKey["api-key"] = "api key";

            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "Chinedu Anulugwo";
            string SenderEmail = "chineduanulugwo@gmail.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            string ToEmail = user.Email.ToString().ToLower();
            string ToName = user.UserName.ToString();
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(ToEmail, ToName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
            string HtmlContent = null;
            string TextContent = emailContent;
            string Subject = "Email Confirmation";            

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
            return true;
        }

        public bool EmailConfirmationStatus()
        {
            var userId = _userManager.GetUserId(_contextAccessor.HttpContext.User);
            var user = _generalPollsRepository.GetUser(userId);
            return (user.EmailConfirmed);

        }
        public ApplicationUser GetUser()
        {
            var userId = _userManager.GetUserId(_contextAccessor.HttpContext.User);
            ApplicationUser user = _generalPollsRepository.GetUser(userId);
            return user;
        }

        // public async Task<bool> EmailConfirmationTrue()
        // {
        //    var confirmation = await _generalPollsRepository.EmailConfirmationTrue();
        //    return confirmation; 
        // }
    }
}
