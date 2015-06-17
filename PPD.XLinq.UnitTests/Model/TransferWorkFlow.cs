using PPD.XLinq.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.UnitTests.Model
{
    [Table("ppdai","TransferWorkFlows")]
    //[Table("TransferWorkFlows")]
    public class TransferWorkFlow
    {
        public int UploadUserId { get; set; }
        public string UploadFileName { get; set; }
    }
}
