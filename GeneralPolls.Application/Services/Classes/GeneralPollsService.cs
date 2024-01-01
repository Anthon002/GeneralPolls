using GeneralPolls.Application.IRepositories;
using GeneralPolls.Application.Services.Interfaces;
using GeneralPolls.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Application.Services.Classes
{
    public class GeneralPollsService: IGeneralPolls
    {
        private readonly IGeneralPollsRepository _generalpollsrepository;
        public GeneralPollsService(IGeneralPollsRepository generalPollsRepository)
        {
            _generalpollsrepository = generalPollsRepository;
        }
        public Task<List<PollsViewModel>> ViewPolls()
        {
            return _generalpollsrepository.ViewPolls();
        }

        public Task<PollsViewModel> CreateNewPoll(PollsViewModel newPoll)
        {
            if (newPoll == null)
            {
                return (null);
            }
            return _generalpollsrepository.CreateNewPoll(newPoll);
        }
    }
}
