using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Players")]
	public class Player
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Username { get; set; }
		public virtual string Password { get; set; }
		public virtual string Email { get; set; }
		public virtual bool IsEnabled { get; set; }
		public virtual DateTime CreateTime { get; set; }
		public virtual DateTime? UpdateTime { get; set; }
}
}

	