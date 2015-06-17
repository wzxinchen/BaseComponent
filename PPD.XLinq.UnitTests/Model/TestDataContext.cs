using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.UnitTests.Model
{
    public class TestDataContext : DataContext
    {
        public TestDataContext()
            : base("test")
        {

        }

        public virtual DbSet<User> Users { get; set; }
       public virtual DbSet<TransferOrder> TransferOrders { get; set; }
        public virtual DbSet<TransferWorkFlow> TransferWorkFlows { get; set; }
    }
}
