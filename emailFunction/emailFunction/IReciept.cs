using System;
namespace emailFunction
{
    public interface IReciept
    {
        int OrderId { get; set; }
        string Email { get; set; }
        string DepSt { get; set; }
        string DesSt { get; set; }
        DateTime DepDateTime { get; set; }
        DateTime DesDateTime { get; set; }

    }
}
