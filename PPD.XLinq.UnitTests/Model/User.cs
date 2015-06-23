using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.UnitTests.Model
{

    [PPD.XLinq.Attributes.Table("ppdai","User")]
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
    }
}
