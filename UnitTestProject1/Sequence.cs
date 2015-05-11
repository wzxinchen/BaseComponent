using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Sequences")]
	public class Sequence
	{
				public virtual string Name { get; set; }
		public virtual int Count { get; set; }
[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
}
}

	