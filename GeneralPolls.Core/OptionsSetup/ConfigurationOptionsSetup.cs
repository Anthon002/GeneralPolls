using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralPolls.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace GeneralPolls.Core.OptionsSetup
{
    public class ConfigurationOptionsSetup : IConfigureOptions<ApiKeysOptions>
    {
        private readonly IConfiguration _configuration;
        public ConfigurationOptionsSetup(IConfiguration configuration)
        {
           _configuration = configuration; 
        }
        public void Configure(ApiKeysOptions options)
        {
             _configuration.GetSection("ApiKeys").Bind(options);
        }
    }
}