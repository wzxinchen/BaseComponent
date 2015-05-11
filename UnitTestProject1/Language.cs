using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Languages")]
	public class Language
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
}
}

	