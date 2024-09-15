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
        public string UserId{get;set;}
        public DateTime DateCreated{get; set;}
        public DateTime EndDate {get; set;}
    }
}
