using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AssignRevokeUser
{
    public static class AssignRevokeUserRequest
    {
        [FunctionName("AssignRevokeUserRequest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {

            List<UserRequest> lst = null;
            string EmployeeId = string.Empty;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            EmployeeId = data?.employeeId;


            if (EmployeeId != null)
            {
                lst = new List<UserRequest>();
                for (int i = 0; i < 1; i++)
                {
                    lst.Add(new UserRequest() { EmployeeId = "1000038829", Name = "Satish Rathore", Dept = "ATM", From = DateTime.Now.AddDays(-i), To = DateTime.Now.AddDays(-i + 7) });
                }

                return (ActionResult)new OkObjectResult(lst);
            }

            return new BadRequestObjectResult("Please pass a employee Id in the request body");
        }

        //public static async Task<IActionResult> Run(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        //    ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}
    }
}
