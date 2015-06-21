using PPD.XLinq.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.UnitTests.Model
{
    [Table("ppdai", "TransferOrders")]
    //[Table("TransferOrders")]
    public class TransferOrder
    {
        public int Id { get; set; }
        public int ToUserId { get; set; }
        public string ToUsername { get; set; }
    }
}
