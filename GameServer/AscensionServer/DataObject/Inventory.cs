using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class Inventory
    {
        public virtual int RoleId { set; get; }
        public virtual string ItemDict{ set; get; }

        public Inventory()
        {
            RoleId = -1;
            ItemDict = "{}";
        }
    }
}
