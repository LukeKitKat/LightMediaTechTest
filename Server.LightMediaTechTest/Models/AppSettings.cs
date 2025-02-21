using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.Models
{
    public class AppSettings
    {
        private readonly IConfiguration _configuration;
        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            var settingsSection = _configuration.GetRequiredSection("AppSettings");

            ConnectionString = settingsSection["ConnectionString"];
        }

        public string? ConnectionString { get; set; }
    }
}
