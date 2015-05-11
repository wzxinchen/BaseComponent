using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("TriggerTemplete")]
	public class TriggerTemplete
	{
		[Key]
		[AutoIncrement]
		public virtual int SysNo { get; set; }
		public virtual string TempleteName { get; set; }
		public virtual string QuerySql { get; set; }
		public virtual string ContentSql { get; set; }
		public virtual string ContentResult { get; set; }
		public virtual int? Status { get; set; }
		public virtual string DataSql { get; set; }

        public virtual TriggerLog Log { get; set; }
}
}
	