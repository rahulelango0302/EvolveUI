using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvolveWebApp
{
    public class UserRequest
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
