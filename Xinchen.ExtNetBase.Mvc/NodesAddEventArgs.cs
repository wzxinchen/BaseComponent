using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ExtNetBase.Mvc
{
    public class NodesAddEventArgs:EventArgs
    {
        public NodesAddEventArgs()
        {
            NodeIds = new List<int>();
        }
        public List<int> NodeIds { get; private set; }

        public bool CancelAdd { get; set; }

        public string ErrorMessage { get; set; }
    }
}
