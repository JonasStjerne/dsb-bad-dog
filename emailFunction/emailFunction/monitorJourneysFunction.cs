using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using emailFunction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace emailFunction
{
    public static class monitorJourneysFunction
    {
        [FunctionName("monitorJourneysFunction")]
        public static async void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var delayTriggerInMinutes = 30;
            List<Receipt> ElapsedTravels;
            DateTime time = DateTime.Now;

            // Get the connection string from app settings and use it to create a connection.
            var str = Environment.GetEnvironmentVariable("sqldb_connection");

            var commandText = String.Format("SELECT * FROM dbo.Reciept WHERE arrTime < {0}", time.AddMinutes(-delayTriggerInMinutes).ToString("HH:mm"));

            using (SqlConnection conn = new SqlConnection(str))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    try
                    {
                        conn.Open();

                        // Execute the command.
                        ElapsedTravels = await cmd.ExecuteNonQueryAsync();

                        foreach (var journey in ElapsedTravels)
                        {
                            //Get ID's of depature station and destination station
                            int originId = await RejseplanenService.LocationSerivce(journey.depStation);
                            int destId = await RejseplanenService.LocationSerivce(journey.arrStation);

                            //Get the link/request to journey that best matches the trip criteria
                            var journeyInformationReq = await RejseplanenService.TripService(originId, destId, journey.arrTime.ToString("HH:mm"), journey.depTime.ToString("dd.MM.yy"));

                            //Get more information about the specific journey found above
                            dynamic journeyResponse = await RejseplanenService.JourneyDetailService(journeyInformationReq);

                            //Find the destination station and retrieve the real time data. If the train is more than delayTrigger start refund otherwise ignore
                            foreach (var stop in journeyResponse)
                            {
                                if (stop.name == journey.arrStation)
                                {
                                    
                                    if (stop.rtTime)
                                    {
                                        DateTime rtData = DateTime.Parse(stop.rtTime);
                                        DateTime plannedArr = DateTime.Parse(stop.time);

                                        //Trigger refund if realtime data display more than delayTrigger delay and is at destinantion within 5 minutes
                                        if (rtData > plannedArr.AddMinutes(delayTriggerInMinutes) && rtData < DateTime.Now.AddMinutes(5))
                                        {
                                            getRefund(journey.OrderId);
                                            //Remember to delete Reciept from database when refund complete.
                                        }
                                    }
                                    //If no realtime data, the train has arrived before delayTrigger, therefor delete
                                    else
                                    {
                                        //Delete receipt, train arrived before delay trigger
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex.ToString());
                    }
                }
            }
        }
    }
}
