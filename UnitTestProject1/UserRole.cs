using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("UserRoles")]
	public class UserRole
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int UserId { get; set; }
		public virtual int RoleId { get; set; }
}
}

	