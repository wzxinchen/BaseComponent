using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("DependenceTasks")]
	public class DependenceTask
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int TaskId { get; set; }
		public virtual int DependenceTaskId { get; set; }
}
}

	