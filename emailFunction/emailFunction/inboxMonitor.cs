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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Reciept>(requestBody);
            data.price = data.price / 100;

            using (SqlConnection conn = new SqlConnection(str))
            {

                var commandText = "Insert into dbo.Reciept (depStation, arrStation, date, depTime, arrTime, price, orderId)" +
                    "value (@depStation, @arrStation, @date, @depTime, @arrTime, @price, @orderId)";

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    SqlParameter depStation = new SqlParameter("@depStation", System.Data.SqlDbType.VarChar);
                    depStation.Value = data.depStation;
                    cmd.Parameters.Add(depStation);

                    SqlParameter arrStation = new SqlParameter("@arrStation", System.Data.SqlDbType.VarChar);
                    arrStation.Value = data.arrStation;
                    cmd.Parameters.Add(arrStation);

                    SqlParameter date = new SqlParameter("@date", System.Data.SqlDbType.Date);
                    date.Value = data.date;
                    cmd.Parameters.Add(date);

                    SqlParameter depTime = new SqlParameter("@depTime", System.Data.SqlDbType.Time);
                    depTime.Value = data.depTime;
                    cmd.Parameters.Add(depTime);

                    SqlParameter arrTime = new SqlParameter("@arrTime", System.Data.SqlDbType.Time);
                    arrTime.Value = data.arrTime;
                    cmd.Parameters.Add(arrTime);

                    SqlParameter price = new SqlParameter("@price", System.Data.SqlDbType.Int);
                    price.Value = data.price;
                    cmd.Parameters.Add(price);

                    SqlParameter orderId = new SqlParameter("@orderId", System.Data.SqlDbType.Int);
                    orderId.Value = data.orderId;
                    cmd.Parameters.Add(orderId);


                    try
                    {
                        conn.Open();

                        // Execute the command.
                        await cmd.ExecuteNonQueryAsync();
                        log.LogInformation("New reciept added");
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex.ToString());
                    }
                }



                //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                //var data = JsonConvert.DeserializeObject<Reciept>(requestBody);
                //data.price = data.price / 100;
                //log.LogInformation(data.price.ToString());

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