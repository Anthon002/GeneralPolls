using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPolls.Core.DTOs;

namespace GeneralPolls.Application.Services.Interfaces
{
    public interface IGeneralPolls
    {
        Task<List<PollsViewModel>> ViewPolls();
        Task<PollsViewModel> CreateNewPoll(PollsViewModel newPoll);
    }
}
