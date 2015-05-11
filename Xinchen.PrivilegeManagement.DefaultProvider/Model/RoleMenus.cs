using System;
using System.ComponentModel.DataAnnotations;
using Xinchen.Utils.DataAnnotations;
namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
	[Table("RoleMenuses")]
    public class RoleMenus
    {
		[Key,AutoIncrement]
public virtual int Id {get;set;}
public virtual int RoleId {get;set;}
public virtual int MenuId {get;set;}
    }
}
