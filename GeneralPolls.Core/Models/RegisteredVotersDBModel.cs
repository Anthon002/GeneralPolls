using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.Models
{
    public class RegisteredVotersDBModel
    {
        public string Id { get; set; }
        public string ElectionId { get; set; }
        public string UserId { get; set; }
        public int Vote { get; set; } = 1;

        /*
         * Voting Operation:
         * User clicks on an Election/Poll they want to observe
         * User clicks on 'Register' to be registered as a voter under that election/poll
         *      If (UserId and ElectionId exists simultaneously){ Doesn't register}
         *      
         */
    }
}
