using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.Controls.Grid
{
    public class ColumnAttributes
    {
        public DataGridColumnAttribute DataGridColumnDisplayAttribute { get; set; }
        public DisplayAttribute DisplayAttribute { get; set; }
        public RequiredAttribute RequiredAttribute { get; set; }
        public RegularExpressionAttribute RegularExpressionAttribute { get; set; }
        public KeyAttribute KeyAttribute { get; set; }
        public PasswordPropertyTextAttribute PasswordPropertyTextAttribute { get; set; }
        public ValidationAttribute ValidationAttribute { get; set; }
        public DefaultValueAttribute DefaultAttribute { get; set; }
        //public DataSourceAttribute DataSourceAttribute { get; set; }
    }
}
