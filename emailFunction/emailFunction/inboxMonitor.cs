using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace emailFunction
{
    public static class inboxMonitor
    {
        [FunctionName("inboxMonitor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            try
            {
                var Reciept = extractEmailContent(data["Text-part"]);
            } catch(InvalidCastException e)
            {
                //If the cast throws exeption log it(a new email layout is properly used)
                log.LogError(e.ToString());
                //return 400
                return new BadRequestResult();
            }
           

            //Insert Reciept into db


            return new OkResult();
        }

        public static IReciept extractEmailContent(dynamic content)
        {
            var orderIdIdentifier = "Dit ordrenummer er:";
            var stationIdentifier = "Til rejsen:";
            var priceIdentifyer = "Pris i alt:";
            var departureTimeIdentifer = "Afgang";
            var arrivalTimeIdentifer = "Ankomst";

            return (IReciept)content;
        }
    }

    
}
