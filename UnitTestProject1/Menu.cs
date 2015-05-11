using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("Menus")]
	public class Menu
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual int Status { get; set; }
		public virtual int Sort { get; set; }
		public virtual int ParentId { get; set; }
		public virtual string Url { get; set; }
		public virtual int? PrivilegeId { get; set; }
}
}

	