using EvolveWebApp.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.ExternalConnectors;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace EvolveWebApp
{
    public class FormDB
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        public static IConfigurationRoot Configuration;

        public  FormDB() {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.Development.json");

            Configuration = builder.Build();
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];



     
            sqlConnection = new SqlConnection(connectionString);
          

        }

        public void Save(FormModel formModel)
        {
            sqlConnection.Open();
            sqlCommand=new SqlCommand("InsertEmpData",sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@EmpName", formModel.Name);
            sqlCommand.Parameters.AddWithValue("@EmpID", formModel.EmpId);
            sqlCommand.Parameters.AddWithValue("@EmpObjectID", formModel.ObjectId);
            sqlCommand.Parameters.AddWithValue("@EmpDeptName", formModel.Dept);
            sqlCommand.Parameters.AddWithValue("@TrainingName", formModel.TrainingName);
            sqlCommand.Parameters.AddWithValue("@TicketNo", formModel.TicketNumber);
            sqlCommand.Parameters.AddWithValue("@VDIStartDate", formModel.StartDate);
            sqlCommand.Parameters.AddWithValue("@VDIEndDate", formModel.EndDate);
       
            sqlCommand.Parameters.AddWithValue("@Location", formModel.Location);
            sqlCommand.Parameters.AddWithValue("@Comment", formModel.Comments);
            sqlCommand.Parameters.AddWithValue("@TrainerName", formModel.TrainerName);
            sqlCommand.Parameters.AddWithValue("@GrpObjectId", formModel.SecurityGroup);
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

    }
}
