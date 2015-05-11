using System;
using System.ComponentModel.DataAnnotations;
using Xinchen.Utils.DataAnnotations;
namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
	[Table("DepartmentPrivileges")]
    public class DepartmentPrivilege
    {
		[Key]
public virtual int Id {get;set;}
public virtual int DepartmentId {get;set;}
public virtual int PrivilegeId {get;set;}
    }
}
