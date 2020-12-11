using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class Inventory
    {
        public virtual int RoleID { set; get; }
        public virtual string ItemDict{ set; get; }

        public Inventory()
        {
            RoleID = -1;
            ItemDict = "{}";
        }
    }
}
