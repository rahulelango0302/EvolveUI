using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EvolveWebApp.Pages
{
    public class DeleteLabModel : PageModel
    {
        private readonly ILogger<DeleteLabModel> _logger;
        public List<UserDashboard> emp;
        public DeleteLabModel(ILogger<DeleteLabModel> logger)
        {
            _logger = logger;
            emp = new List<UserDashboard>()
            {
                new UserDashboard()
                {
                    EmployeeId = "1000038829",
                    Name="Satish Rathore",
                    Dept="ATMMSCOMP",
                    AccountName="BBH",
                    From = DateTime.Now,
                    To = DateTime.Now,
                    ProjectName="IM System",
                    Location = "Mumbai",
                    Grade = "G6",
                    CPUUsage = "30%",
                    MemoryUsage ="5%",
                    Cost="$12",
                    Status = true
                },

                new UserDashboard()
                {
                    EmployeeId = "1000057360",
                    Name="Danish Shaikh",
                    Dept="IMS",
                    AccountName="BBH",
                    From = DateTime.Now,
                    To = DateTime.Now,
                    ProjectName="PB",
                    Location = "Mumbai",
                    Grade = "G5",
                    CPUUsage = "34%",
                    MemoryUsage ="15%",
                    Cost="$15",
                    Status = true
                },

                 new UserDashboard()
                {
                    EmployeeId = "1000027560",
                    Name="Arun Prasaath",
                    Dept="BFS",
                    AccountName="BBH",
                    From = DateTime.Now,
                    To = DateTime.Now,
                    ProjectName="QLIKM",
                    Location = "Channai",
                    Grade = "G6",
                    CPUUsage = "41%",
                    MemoryUsage ="35%",
                    Cost="$35",
                    Status = true
                },

                   new UserDashboard()
                {
                    EmployeeId = "1000057560",
                    Name="Shalu Choudhary",
                    Dept="ATMJAVA",
                    AccountName="BBH",
                    From = DateTime.Now,
                    To = DateTime.Now,
                    ProjectName="IM",
                    Location = "Noida",
                    Grade = "G6",
                    CPUUsage = "11%",
                    MemoryUsage ="5%",
                    Cost="$5",
                    Status = false
                },
            };
        }
        public void OnGet()
        {
            ViewData["empData"] = emp.ToList();
        }
    }
}
