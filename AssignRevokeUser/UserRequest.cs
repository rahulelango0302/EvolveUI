using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignRevokeUser
{
    public class UserRequest
    {
        [JsonProperty(PropertyName = "employeeId")]
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
