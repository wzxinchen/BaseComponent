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
    [Table("TransferWorkFlows")]
    //[Table("TransferWorkFlows")]
    public class TransferWorkFlow
    {
        public int UploadUserId { get; set; }
        public string UploadFileName { get; set; }
    }
}
