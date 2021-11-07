using System;
using System.Collections.Generic;
using System.Text;

namespace service.Models
{
    public class JourneyDetailServiceResponse
    {
        public string noNamespaceSchemaLocation { get; set; }
        public IList<StStopInformation> Stop { get; set; }

    }
}
