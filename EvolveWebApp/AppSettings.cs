using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvolveWebApp
{
    public class AppSettings
    {
        public string AzureFunctionURL { get; set; }
        public string DefaultConnection { get; set; }

        public string FromEmail { get; set; }

    }
}
