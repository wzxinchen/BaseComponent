using ConsoleApplication1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbUtils.DataAnnotations;
using Xinchen.Utils.DataAnnotations;

namespace PPD.XLinq.UnitTests.Model
{
    [DataBase("ppdai")]
    [Table("User")]
    //[Table("User")]
    public class User
    {
        [Column("UserId"),Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual bool IsEnabled { get; set; }
        //public DateTime? VerifyTime { get; set; }
        //public DateTime RegTime { get; set; }
        //public int Age { get; set; }

        public virtual DateTime? LastLoginDate { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual T Status { get; set; }
    }
}
