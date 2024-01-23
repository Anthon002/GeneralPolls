﻿using GeneralPolls.Core.DTOs;
using GeneralPolls.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Application.IRepositories
{
    public interface IGeneralPollsRepository
    {
        Task<List<PollsViewModel>> ViewPolls();
        Task<PollsViewModel> CreateNewPoll( PollsViewModel newPoll);
        Task<CandidateViewModel> AddCandidate( CandidateDBModel newCandidate );
        Task<IEnumerable<CandidateViewModel>> ViewRegisteredCandidates(string ElectionId);
    }
}
