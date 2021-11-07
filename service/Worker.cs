using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private List<Reciept> ElapsedTravels;
        private int delayTriggerInMinutes = 30;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime time = DateTime.Now;

                using (var context = new dsb_bad_dogContext())
                {
                    //Returns all the travels that ended more than 30 minutes ago
                    ElapsedTravels = context.Reciepts.Where(travelInfo => travelInfo.DesDateTime.AddMinutes(delayTriggerInMinutes) < time).ToList();
                }
                //For each of the journeys that ended more than 30 minutes ago
                foreach (var journey in ElapsedTravels)
                {
                    Console.WriteLine("Ran");
                    //Get ID's of depature station and destination station
                    int originId = await RejseplanenService.LocationSerivce(journey.DepSt);
                    int destId = await RejseplanenService.LocationSerivce(journey.DesSt);
                    Console.WriteLine("originId: " + originId + "   destId: " + destId);
                    Console.WriteLine(journey.DepDateTime.ToString("HH:mm") + journey.DepDateTime.ToString("dd.MM.yy"));

                    //Get the link/request to journey that best matches the trip criteria
                    var journeyInformationReq = await RejseplanenService.TripService(originId, destId, journey.DepDateTime.ToString("HH:mm"), journey.DepDateTime.ToString("dd.MM.yy"));
                    Console.WriteLine("JourneyInformation: " + journeyInformationReq);

                    //Get more information about the specific journey found above
                    dynamic journeyResponse = await RejseplanenService.JourneyDetailService(journeyInformationReq);
           
                    foreach (var stop in journeyResponse)
                    {
                        if (stop.name == journey.DesSt)
                        {
                            
                            Console.WriteLine("found this stop matching the stopping station: " + stop);
                        if (stop.rtTime)
                            {
                                DateTime rtData = DateTime.Parse(stop.rtTime);
                                DateTime plannedArr = DateTime.Parse(stop.time);
                                if (rtData > plannedArr.AddMinutes(delayTriggerInMinutes) && rtData < DateTime.Now.AddMinutes(5))
                                {
                                    getRefund(journey.OrderId);
                                }
                            }
                            break;
                        }
                    }
                }
    




                //for (int i = 0; i < journeyResponse.length; i++)
                //{
                //    Console.WriteLine("checking for stop");
                //    if (journeyResponse[i].name == "aalborg st.")
                //    {
                //        var stopinformation = journeyResponse[i];
                //        Console.WriteLine("found this stop matching the stopping station: " + stopinformation);
                //        break;
                //    }
                //}










                //For each of the journeys that ended more than 30 minutes ago
                foreach (var journey in ElapsedTravels)
                {
                    //Get the stations id's from the rejseplan API
                    //int originId = await RejseplanenService.LocationSerivce(journey.DepSt);
                    //int destId = await RejseplanenService.LocationSerivce(journey.DesSt);
                    //Console.WriteLine("originId: " + originId + "   destId: " + destId);

                    ////Get the a list of journeys wiith link which includes the specific train the stopping stations with arrival- and depature times includin realtime data
                    //var journeyInformation = await RejseplanenService.TripService(originId, destId, journey.DepDateTime, journey.DepDateTime);

                    ////Extract the first journey in the list and extract the ref to the data for the specifik journey
                    //var journeyDetailsRef = journeyInformation.TripList.Trip[0].JourneyDetailRef["ref"];

                    ////Get details of the specific journey
                    //TripServiceResponse response = await RejseplanenService.JourneyDetailService(journeyDetailsRef);
                    //Console.WriteLine("Response from Journeydetail: " + response);
                    //var test = response.JourneyDetail.Stop.Find(x => x.name == journey.DepSt);
                    //Console.WriteLine("Found this stop matching the stopping station: " + test);

                    //If there is a delay at the finalDestination and it's more than 30 mins from the planned arrival and the journey ends within 5 minutes start refund.
                    //if ( journeyStoppingData.Stop[name=journey.DestSt].rtAarTime > journeyStoppingData.Stop[name=journey.DestSt].AarTime.AddMinutes(delayTriggerInMinutes && journeyStoppingData.Stop[name=journey.DestSt].rtAarTime < time.AddMinutes(5)) {
                    //  RefundTicket(journey);
                    //}
                    //context.Reciepts.remove(journey);
                    //context.SaveChanges();
                }




                //string response = await RejseplanenService.LocationSerivce("Skanderborg St.");

                //foreach (var journey in ElapsedTravels)
                //{

                //    //if (haveBeenAtdesSt) {
                //    //  actualDesDateTime = rejseplanenRequest()
                //    //  if(actualDesDateTime > jouney.DesDateTime.addMinutes(delayTriggerMinutes)) {
                //    //     RefundTicket(journey) 
                //    //  }
                //    //  context.Reciepts.remove(journey)
                //    //}
                //
                await Task.Delay(10 * 1000, stoppingToken);
            }
        }

        private void getRefund(int orderId)
        {
            Console.WriteLine("Refund startet");
        }
    }
}
