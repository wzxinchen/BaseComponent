using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils.DataAnnotations
{
    public class PrivilegeDescriptionAttribute:Attribute
    {
        //public PrivilegeDescriptionAttribute(string desc,int menuId)
        //{
        //    Description = desc;
        //    MenuId = menuId;
        //}
        public PrivilegeDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
        //public int? MenuId { get; set; }
    }
}
