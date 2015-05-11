using System;
using Xinchen.Utils.DataAnnotations;
using System.ComponentModel.DataAnnotations;
namespace UnitTestProject1
{
	[Table("TaskExampleLanguages")]
	public class TaskExampleLanguage
	{
		[Key]
		[AutoIncrement]
		public virtual int Id { get; set; }
		public virtual int TaskId { get; set; }
		public virtual int LanguageId { get; set; }
		public virtual string Code { get; set; }
}
}

	