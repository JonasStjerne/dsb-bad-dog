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
using Newtonsoft.Json.Converters;

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
            Receipt data = new Receipt();

            try
            {
                data = JsonConvert.DeserializeObject<Receipt>(requestBody, new JsonSerializerSettings()
                { Culture = new System.Globalization.CultureInfo("da-DK") });
            } catch (Exception ex)
            {
                return new BadRequestResult();
            }

            data.price = data.price / 100;


            using (SqlConnection conn = new SqlConnection(str))
            {

                var commandText = "Insert into dbo.Reciept (depStation, arrStation, depDate, depTime, arrTime, price, orderId)" +
                    "values (@depStation, @arrStation, @date, @depTime, @arrTime, @price, @orderId)";

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    
                    cmd.Parameters.Add(createSqlParam("@depStation", System.Data.SqlDbType.VarChar, data.depStation));
                    cmd.Parameters.Add(createSqlParam("@arrStation", System.Data.SqlDbType.VarChar, data.arrStation));
                    cmd.Parameters.Add(createSqlParam("@date", System.Data.SqlDbType.Date, data.date));
                    cmd.Parameters.Add(createSqlParam("@depTime", System.Data.SqlDbType.Time, data.depTime));
                    cmd.Parameters.Add(createSqlParam("@arrTime", System.Data.SqlDbType.Time, data.arrTime));
                    cmd.Parameters.Add(createSqlParam("@price", System.Data.SqlDbType.Int, data.price));
                    cmd.Parameters.Add(createSqlParam("@orderId", System.Data.SqlDbType.Int, data.orderId));

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
                        return new BadRequestResult();
                    }
                }

                return new OkResult();
            }

        }

        public class Receipt
        {
            public string depStation { get; set; }
            public string arrStation { get; set; }
            public DateTime date { get; set; }
            public TimeSpan depTime { get; set; }
            public TimeSpan arrTime { get; set; }
            public Double price { get; set; }
            public string orderId { get; set; }
        }

        public static SqlParameter createSqlParam(string paramBind, System.Data.SqlDbType dbParamType, dynamic data)
        {
            SqlParameter param = new SqlParameter(paramBind, dbParamType);
            param.Value = data;
            return param;
        }
    }
}