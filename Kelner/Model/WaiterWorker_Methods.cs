using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kelner.Model
{
    public partial class WaiterWorker
    {
        private int CheckFreeTables()
        {
            var random = new Random();
            return random.Next(1, 4);
        }

        private int CheckDirtyTables()
        {
            var random = new Random();
            return random.Next(1, 4);
        }

        private int WaitingTime()
        {
            var random = new Random();
            return random.Next(0, 60);
        }

        private int OrdersOnKitchen()
        {
            var random = new Random();
            return random.Next(0, 5);
        }
    }
}
