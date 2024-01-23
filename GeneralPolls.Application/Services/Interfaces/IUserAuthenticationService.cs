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
        Task<RegistrationViewModel> Register(RegistrationViewModel newUser);
        Task<LoginViewModel> Login(LoginViewModel user);
        Task<IEnumerable<CandidateViewModel>> GetUsers(string Id);

    }
}
