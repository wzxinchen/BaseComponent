using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.PrivilegeManagement.DTO
{
    [Table("Sequences")]
    public class Sequence
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
    }
}
