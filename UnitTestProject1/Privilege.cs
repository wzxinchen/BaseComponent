using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Privileges")]
	public class Privilege
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
}
}

	