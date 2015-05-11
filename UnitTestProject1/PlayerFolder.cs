using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("PlayerFolders")]
	public class PlayerFolder
	{
		[Key]
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int ParentId { get; set; }
		public virtual int PlayerId { get; set; }
}
}

	