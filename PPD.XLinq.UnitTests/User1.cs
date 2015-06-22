using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPD.XLinq.UnitTests
{
    public class User1
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsEnabled1 { get; set; }
        //public DateTime? VerifyTime { get; set; }
        //public DateTime RegTime { get; set; }
        //public int Age { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}
