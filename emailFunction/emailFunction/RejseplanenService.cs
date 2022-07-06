using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using service.Models;
using System.Data;

namespace service
{
    class RejseplanenService
    {
        private static readonly HttpClient client = new HttpClient();
        public int locationId { get; set; }

        public static async Task<int> LocationSerivce(string stationString) {
            string response = await client.GetStringAsync("http://xmlopen.rejseplanen.dk/bin/rest.exe/location?format=json&input=" + stationString);
            var locationData = JsonConvert.DeserializeObject<dynamic>(response);
            return locationData.LocationList.StopLocation[0].id;
        }

        public static async Task<string> TripService(int originId, int destId, string time, string date)
        {
            var response = await client.GetStringAsync(String.Format("http://xmlopen.rejseplanen.dk/bin/rest.exe/trip?format=json&originId={0}&destId={1}&useBus=0&useMetro=0&date={2}&time={3}", originId, destId, date, time));
            var tripData = JsonConvert.DeserializeObject<dynamic>(response);
            return tripData.TripList.Trip[0].Leg.JourneyDetailRef["ref"];
        }

        public static async Task<dynamic> JourneyDetailService(string req)
        {
            var response = await client.GetStringAsync(req);
            var DesResponse = JsonConvert.DeserializeObject<dynamic>(response);
            return DesResponse.JourneyDetail.Stop;
        }
    }
}
