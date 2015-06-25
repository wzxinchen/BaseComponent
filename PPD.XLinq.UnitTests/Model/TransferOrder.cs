using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbUtils.DataAnnotations;
using Xinchen.Utils.DataAnnotations;

namespace PPD.XLinq.UnitTests.Model
{
    [DataBase("ppdai")]
    [Table("TransferOrders")]
    //[Table("TransferOrders")]
    public class TransferOrder
    {
        public int Id { get; set; }
        public int ToUserId { get; set; }
        [Column("ToUserName")]
        public string ToUsername { get; set; }
    }
}
