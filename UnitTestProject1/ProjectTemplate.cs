using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("ProjectTemplates")]
	public class ProjectTemplate
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual bool IsEnabled { get; set; }
		public virtual DateTime CreateTime { get; set; }
		public virtual DateTime? UpdateTime { get; set; }
}
}

	