using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Tasks")]
	public class Task
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int StagesId { get; set; }
		public virtual DateTime CreateTime { get; set; }
		public virtual DateTime? UpdateTime { get; set; }
		public virtual bool IsEnabled { get; set; }
		public virtual string Objectives { get; set; }
		public virtual string Introduce { get; set; }
		public virtual string ExecuteContent { get; set; }
		public virtual string Example { get; set; }
		public virtual int CheckType { get; set; }
		public virtual string CheckAssembly { get; set; }
}
}

	