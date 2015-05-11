using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("DependenceStageses")]
	public class DependenceStages
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int StagesId { get; set; }
		public virtual int DependenceStagesId { get; set; }
}
}

	