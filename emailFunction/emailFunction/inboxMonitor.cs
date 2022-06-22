using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;


namespace emailFunction
{
    public static class inboxMonitor
    {
        [FunctionName("inboxMonitor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the connection string from app settings and use it to create a connection.
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection conn = new SqlConnection(str))
            {
               
                var commandText = "Insert into dbo.Reciept (depStation, arrStation, date, depTime, arrTime, price, orderId)" +
                    "value (@depStation, @arrStation, @date, @depTime, @arrTime, @price, @orderId)";

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {

                    cmd.Parameters.Add("@depStation", SqlDbType.Int);
                    try
                    {
                        conn.Open();

                        // Execute the command and log the # rows affected.
                        await cmd.ExecuteNonQueryAsync();
                        log.LogInformation("New reciept added");
                    } catch(Exception ex)
                    {
                        log.LogError(ex.ToString());
                    }
                }



            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Reciept>(requestBody);
            data.price = data.price / 100;
            log.LogInformation(data.price.ToString());

            //Insert Reciept into db


            return new OkResult();
        }

    }

    public class Reciept
    {
        public string depStation { get; set; }
        public string arrStation { get; set; }
        public string date { get; set; }
        public string depTime { get; set; }
        public string arrTime { get; set; }
        public int price { get; set; }
        public string orderId { get; set; }
    }
}
