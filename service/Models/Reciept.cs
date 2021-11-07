using System;
using System.Collections.Generic;

#nullable disable

namespace service.Models
{
    public partial class Reciept
    {
        public int OrderId { get; set; }
        public string Email { get; set; }
        public string DepSt { get; set; }
        public string DesSt { get; set; }
        public DateTime DepDateTime { get; set; }
        public DateTime DesDateTime { get; set; }
    }
}
