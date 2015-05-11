using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("RoleMenus")]
	public class RoleMenu
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int RoleId { get; set; }
		public virtual int MenuId { get; set; }
}
}

	