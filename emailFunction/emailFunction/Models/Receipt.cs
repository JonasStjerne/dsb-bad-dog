using System;
namespace emailFunction.Models
{
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
}
