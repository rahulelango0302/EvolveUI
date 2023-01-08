using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EvolveWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly GraphServiceClient _graphServiceClient;
        private readonly IConfiguration _configuration;
        private AppSettings _appSettings { get; set; }
        private List<EvolveEnvironment> environment = new List<EvolveEnvironment>();
        private List<EvolveSecurityGroup> securityGroup = new List<EvolveSecurityGroup>();
        public IndexModel(ILogger<IndexModel> logger, GraphServiceClient graphServiceClient,
            IOptions<AppSettings> settings, IConfiguration configuration, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
            _appSettings = settings.Value;
            _configuration = configuration;
            _tokenAcquisition = tokenAcquisition;
        }

        private void GetDBObject()
        {
            string constr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT  [EnvironmentID] ,[EnvironmentName] FROM [dbo].[Environment]";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            environment.Add(new EvolveEnvironment
                            {
                                EnvironmentName = sdr["EnvironmentName"].ToString(),
                                EnvironmentID = Convert.ToInt32(sdr["EnvironmentID"])
                            });
                        }
                    }
                    con.Close();
                }

                query = "SELECT [GroupID],[GroupName],[GroupObjectID],[GroupOwner],[IsActive] FROM [dbo].[SecurityGroup]";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            securityGroup.Add(new EvolveSecurityGroup
                            {
                                GroupName = sdr["GroupName"].ToString(),
                                GroupID = Convert.ToInt32(sdr["GroupID"]),
                                GroupObjectID = sdr["GroupObjectID"].ToString(),
                            });
                        }
                    }
                    con.Close();
                }
            }

            TempData["environment"] = environment;
            TempData["securityGroup"] = securityGroup;
        }
        public void OnGet()
        {
            ViewData["successMsg"] = null;
            ViewData["UserRequest"] = null;
        }

        private async Task<bool> checkIfUserExist(string objectId, string grpID)
        {
            bool IsExist = false;
            var users = await _graphServiceClient.Groups[grpID].Members.Request().GetAsync();

            foreach (var user in users)
            {
                if (user.Id == objectId)
                {
                    IsExist = true;
                }
            }
            return IsExist;
        }
        public async Task<IActionResult> OnPostAssign(string objectId, string grpID)
        {
            if (string.IsNullOrEmpty(grpID))
            {
                TempData["successMsg"] = "Select Security Group";
                return RedirectToPage();
            }

            bool isUserExist = await checkIfUserExist(objectId, grpID);

            if (!isUserExist)
            {
              
                User userToAdd = await _graphServiceClient.Users[objectId].Request().GetAsync();
                await _graphServiceClient.Groups[grpID].Members.References.Request().AddAsync(userToAdd);

                TempData["successMsg"] = TempData["employeeID"] + " added successfully, An email was sent with instructions for accessing lab !!";
            }
            else
            {
                TempData["successMsg"] = TempData["employeeID"] + " already exist in group !!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemove(string objectId, string grpID)
        {
            if (string.IsNullOrEmpty(grpID))
            {
                TempData["successMsg"] = "Select Security Group";
                return RedirectToPage();
            }
            bool isUserExist = await checkIfUserExist(objectId, grpID);
            if (isUserExist)
            {               
                await _graphServiceClient.Groups[grpID].Members[objectId].Reference.Request().DeleteAsync();
                TempData["successMsg"] = TempData["employeeID"] + " removed successfully, An email was sent with instructions for accessing lab !!";
            }
            else
            {
                TempData["successMsg"] = TempData["employeeID"] + " not exist in group!!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(string employeeID, string grpID)
        {
            this.GetDBObject();

            if (string.IsNullOrEmpty(employeeID))
            {
                TempData["successMsg"] = "Enter Employee ID";
                return Page();
            }
            //else if (string.IsNullOrEmpty(objectId))
            //{
            //    TempData["successMsg"] = "Select Evolve Environment Type";
            //    return Page();
            //}
            //else if (string.IsNullOrEmpty(grpID))
            //{
            //    TempData["successMsg"] = "Enter Security Group";
            //    return Page();
            //}
            TempData["successMsg"] = null;
            ViewData["UserRequest"] = null;
            ViewData["objectId"] = null;
            TempData["employeeID"] = employeeID;

            var request = await _graphServiceClient.Users
                    .Request()
                    .Filter("employeeId eq '" + employeeID.Trim() + "'")
                    .Select(e => new
                    {
                        e.Id,
                        e.DisplayName,
                        e.Mail,
                        e.UserPrincipalName,
                        e.GivenName,
                        e.Department,
                        e.EmployeeId
                    }).GetAsync();

            var usersDetails = request.CurrentPage.ToList();
            var user = usersDetails.FirstOrDefault();
            if (user != null)
            {
                List<UserRequest> userRequest = new List<UserRequest>();
                for (int i = 0; i < 1; i++)
                {
                    userRequest.Add(new UserRequest()
                    {
                        ObjectId = user.Id,
                        EmployeeId = user.EmployeeId,
                        Name = user.DisplayName,
                        Dept = user.Department,
                        Email = user.UserPrincipalName,
                        From = DateTime.Now.AddDays(-i),
                        To = DateTime.Now.AddDays(-i + 7)
                    });
                }

                ViewData["objectId"] = user.Id;
                ViewData["UserRequest"] = userRequest;
            }
            else
            {
                TempData["successMsg"] = "User not found";
            }

            return Page();

            //var token = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.ReadWrite","User.ReadBasic.All", "User.Read" }).Result;
            //GraphServiceClient graphServiceClientobj = new GraphServiceClient("https://graph.microsoft.com/v1.0/",
            //    new DelegateAuthenticationProvider(
            //        request =>
            //        {
            //            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            //            return Task.CompletedTask;
            //        }));

            //var directoryObject = new DirectoryObject
            //{
            //    Id = "15c3607f-6fab-49a4-9764-9678e90a2d64"
            //};

            //await graphServiceClientobj.Groups["44ae5e55-e838-443d-849e-e40e51dc6316"].Members.References
            //    .Request()
            //    .AddAsync(directoryObject);
            // User userToAdd = await graphServiceClientobj.Users["15c3607f-6fab-49a4-9764-9678e90a2d64"].Request().GetAsync();
            // await graphServiceClientobj.Groups["44ae5e55-e838-443d-849e-e40e51dc6316"].Members.References.Request().AddAsync(userToAdd);

            //var request = await graphServiceClientobj.Users
            //        .Request()
            //        .Filter("employeeId eq '" + employeeID + "'")
            //        .Select(e => new
            //        {
            //            e.Id,
            //            e.DisplayName,
            //            e.Mail,
            //            e.UserPrincipalName,
            //            e.GivenName,
            //            e.Department,
            //            e.EmployeeId
            //        }).GetAsync();

            //var usersDetails = request.CurrentPage.ToList();
            //var user = usersDetails.FirstOrDefault();

            //List<UserRequest> userRequest = new List<UserRequest>();
            //for (int i = 0; i < 1; i++)
            //{
            //    userRequest.Add(new UserRequest() {
            //        ObjectId = user.Id,
            //        EmployeeId = user.EmployeeId,
            //        Name = user.DisplayName, 
            //        Dept = user.Department,
            //        Email = user.UserPrincipalName,
            //        From = DateTime.Now.AddDays(-i), 
            //        To = DateTime.Now.AddDays(-i + 7) });
            //}

            //User userToAdd = await graphServiceClientobj.Users[user.Id].Request().GetAsync();
            //await graphServiceClientobj.Groups["44ae5e55-e838-443d-849e-e40e51dc6316"].Members.References.Request().AddAsync(userToAdd);




            // IGraphServiceUsersCollectionPage users = await graphServiceClientobj.Users.Request().Filter("userPrincipalName eq '1000038829@hexaware.com'").GetAsync(); // Count = 0

            //var user = graphServiceClientobj.Users["15c3607f-6fab-49a4-9764-9678e90a2d64"]
            //        .Request()
            //        .GetAsync();

            //string tenantID = _configuration.GetValue<string>("AzureAD:TenantID");
            //string clientID = _configuration.GetValue<string>("AzureAD:ClientID");
            //string clientSecret = _configuration.GetValue<string>("AzureAD:ClientSecret");
            //ClientSecretCredential clientSecretCred = new ClientSecretCredential(tenantID, clientID, clientSecret);
            //GraphServiceClient graphServiceClientobj = new GraphServiceClient(clientSecretCred);

            //var user = graphServiceClientobj.Users.Request().Select(x => x.DisplayName).GetAsync().Result;


            //var user = await graphServiceClientobj.Users["1000038829"]
            //        .Request()
            //        .GetAsync();

            //if (!string.IsNullOrEmpty(employeeID))
            //{
            //    var Url = _appSettings.AzureFunctionURL;

            //    dynamic content = new { employeeId = employeeID };

            //    //CancellationToken cancellationToken;


            //    using (var client = new HttpClient())
            //    using (var request = new HttpRequestMessage(HttpMethod.Post, Url))
            //    using (var httpContent = CreateHttpContent(content))
            //    {
            //        request.Content = httpContent;

            //        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            //            .ConfigureAwait(false))
            //        {

            //            var resualtList = response.Content.ReadAsAsync<List<UserRequest>>();


            //            ViewData["UserRequest"] = resualtList.Result;

            //            return Page();
            //        }
            //    }
            //}
            //else
            //{
            //    ViewData["UserRequest"] = null;
            //    ViewData["successMsg"] = "Lab created successfully, An email was sent with instructions for accessing lab !!";
            //    return Page();
            //}

        }

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
    }
}
