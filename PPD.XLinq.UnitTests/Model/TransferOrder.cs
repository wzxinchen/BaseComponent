using PPD.XLinq.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.UnitTests.Model
{
    [PPD.XLinq.Attributes.Table("ppdai", "TransferOrders")]
    //[Table("TransferOrders")]
    public class TransferOrder
    {
        public int Id { get; set; }
        public int ToUserId { get; set; }
        [Column("ToUserName")]
        public string ToUsername { get; set; }
    }
}
