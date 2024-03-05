using Microsoft.EntityFrameworkCore;
using GeneralPolls.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GeneralPolls.Infrastructure.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public DbSet<PollsDBModel> PollsTable { get; set; }
        public DbSet<CandidateDBModel> CandidateTable { get; set; }
        public DbSet<RegisteredVotersDBModel> RegisteredVotersTable { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):
            base(options)
        {
            
        }
    }
}
