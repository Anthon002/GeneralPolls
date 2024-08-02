using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;

namespace GeneralPolls.Application.Services.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<string> Register(RegistrationViewModel newUser);
        Task<RegistrationViewModel> AdminRegister(RegistrationViewModel adminUser);
        Task<LoginViewModel> Login(LoginViewModel user);
        Task<IEnumerable<CandidateViewModel>> GetUsers(string Id);
        Task<RegisteredVotersViewModel> GetRegisteredVoter(string ElectionId);
        Task<ApplicationUser> ChangeProfilePicture(ApplicationUser updatedUser);
        Task<String> Logout();
        string GetUserImage(string UserId);
        Task<bool> SendConfirmationLink(string emailContent, ApplicationUser user);
        bool EmailConfirmationStatus();
        ApplicationUser GetUser();
    }
}
