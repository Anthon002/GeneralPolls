using GeneralPolls.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Application.Services.Classes
{
    public class RoleSeederService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleSeederService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task SeedRoles()
        {
            var admin = new IdentityRole("Admin");
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(admin);
            }
        }
        public async Task SeedCustomRoles(string Id)
        {
            var customRole = new IdentityRole(Id);
            if (!await _roleManager.RoleExistsAsync(Id))
            {
                await _roleManager.CreateAsync(customRole);
            }
        }
    }
}
