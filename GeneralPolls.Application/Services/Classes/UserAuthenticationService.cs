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
            var user = new ApplicationUser() { UserName = newUser.FirstName, NormalizedUserName = newUser.FirstName };
            await _userManager.CreateAsync(user, newUser.Password);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return (null);

        }

    }
}
