using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xinchen.Controls
{
    public static class Extension
    {
        public static Control FindControl(this Control control, string name)
        {
            Control _findedControl = null;
            if (!string.IsNullOrEmpty(name) && control != null)
            {
                foreach (Control ctrl in control.Controls)
                {
                    if (ctrl.Name.Equals(name))
                    {
                        _findedControl = ctrl;
                        break;
                    }
                    return ctrl.FindControl(name);
                }
            }
            return _findedControl;
        }

        public static Dictionary<string, Control> GetControls(this Control control)
        {
            Dictionary<string, Control> list = new Dictionary<string, Control>();
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl.GetType().IsPublic)
                {
                    list.Add(ctrl.Name, ctrl);
                    var dict = ctrl.GetControls();
                    foreach (var item in dict)
                    {
                        list.Add(item.Key, item.Value);
                    }
                }
            }
            return list;
        }
    }
}
