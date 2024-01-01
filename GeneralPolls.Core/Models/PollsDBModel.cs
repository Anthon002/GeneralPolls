using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.Models
{
    public class PollsDBModel
    {
        public string Id { get; set; }
        public string ElectionName { get; set; }
        public int CandidateCount { get; set; }
    }
}
