using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace GeneralPolls.Application.Services.Classes
{
    public class UserAuthenticationService: IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserAuthenticationService( UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public async Task<RegistrationViewModel> Register(RegistrationViewModel newUser)
        {
            if (newUser == null)
            {
                return (null);
            }
            var user = new ApplicationUser() { Id = Guid.NewGuid().ToString(), UserName = newUser.Email, Email = newUser.Email, NormalizedEmail = newUser.Email.ToUpper(), FirstName = newUser.FirstName,LastName = newUser.LastName};
            var response = await _userManager.CreateAsync(user, newUser.Password);
            if (response.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return (null);

        }
        public async Task<LoginViewModel> Login(LoginViewModel user)
        {
            var registeredUser = await _userManager.FindByNameAsync(user.Username);
            if (registeredUser == null ){ return (null);}
            var response = _userManager.CheckPasswordAsync(registeredUser, user.Password);
            if (response != null) { return (null);  } 

            await _signInManager.SignInAsync(registeredUser, isPersistent: true);
            return (user);
        }
        public async Task<IEnumerable<CandidateViewModel>> GetUsers(string ElectionId)
        {
            var listofusers = _userManager.Users.Select(x => new CandidateViewModel() { CandidateName = x.FirstName, Email = x.Email, Id = x.Id, ElectionId = ElectionId, VoteCount = 0}).ToList();
            return (listofusers);
        }
    }
}
