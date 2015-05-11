using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Users")]
	public class User
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Username { get; set; }
		public virtual string Password { get; set; }
		public virtual DateTime CreateTime { get; set; }
		public virtual int Status { get; set; }
		public virtual string Description { get; set; }
}
}

	