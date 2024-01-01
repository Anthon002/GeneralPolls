using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.Models
{
    public class CandidateDBModel
    {
        public string Id { get; set;  }
        public string ElectionId { get; set; }
        public int VoteCount { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
    }
}
