using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("RolePrivileges")]
	public class RolePrivilege
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int RoleId { get; set; }
		public virtual int PrivilegeId { get; set; }
}
}

	