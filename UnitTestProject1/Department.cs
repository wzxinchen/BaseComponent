using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Departments")]
	public class Department
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Status { get; set; }
		public virtual string Description { get; set; }
}
}

	