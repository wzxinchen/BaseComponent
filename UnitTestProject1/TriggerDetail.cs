using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
    [Table("TriggerDetail")]
    public class TriggerDetail
    {
        [Key]
        [AutoIncrement]
        public virtual int SysNo { get; set; }

        [ForeignKey(ForeignType = typeof(TriggerTemplete), ForeignProperty = "SysNo", SelfProperty = "TriggerTemplete")]
        public virtual int? TempleteSysNo { get; set; }
        public virtual string DetailName { get; set; }
        public virtual int? SpaceOfTime { get; set; }
        public virtual int? TaskType { get; set; }
        public virtual int? SendType { get; set; }
        public virtual int? ExecType { get; set; }
        public virtual int? DateType { get; set; }
        public virtual int? DateNum { get; set; }
        public virtual string Content { get; set; }
        public virtual string SMSContent { get; set; }
        public virtual int? Status { get; set; }
        public virtual string MailSubject { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string CouponBatchNo { get; set; }
        public virtual string TrackingCode { get; set; }

        public virtual TriggerTemplete TriggerTemplete { get; set; }

        public virtual TriggerTemplete TriggerTemplete1 { get; set; }
    }
}
