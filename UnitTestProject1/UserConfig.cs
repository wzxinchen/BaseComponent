using System;
using Xinchen.DbEntity;
using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("UserConfigs")]
	public class UserConfig
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int UserId { get; set; }
		public virtual int? Mode { get; set; }
		public virtual int? TechnologyType { get; set; }
		public virtual int? ProjectType { get; set; }
}
}

	