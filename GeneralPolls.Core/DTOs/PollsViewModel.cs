using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.DTOs
{
    public class PollsViewModel
    {
        
        public string Id { get; set; }
        [Required]
        public string ElectionName { get; set; }
        public string UserId{get; set;}
    }
}
