using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GAFEAPI.Models
{
    public class IMAPConfiguration
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }

        public IMAPConfiguration(IConfiguration configuration)
        {
            Email = configuration["Email"];
            Password = configuration["Password"];
            Server = configuration["Server"];
            int.TryParse(configuration["Port"], out var port);
            Port = port;
            bool.TryParse(configuration["UseSSL"], out var useSSL);
            UseSSL = useSSL;
        }
    }
}
