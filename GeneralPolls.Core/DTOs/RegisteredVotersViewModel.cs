using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.DTOs
{
    public class RegisteredVotersViewModel
    {
        public string Id { get; set; }
        public string ElectionId { get; set; }
        public string UserId { get; set; }
        public int Vote { get; set; } = 1;
    }
}
