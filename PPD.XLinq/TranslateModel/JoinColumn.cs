using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.TranslateModel
{
    public class JoinColumn : Column
    {
        public JoinType JoinType { get; set; }
        public JoinColumn Next { get; set; }
    }
}
