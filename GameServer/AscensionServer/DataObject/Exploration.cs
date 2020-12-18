using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class Exploration
    {
        public virtual int RoleID { get; set; }
        public virtual string ExplorationItemDict { get; set; }
        public virtual string UnLockDict { get; set; }
        public Exploration()
        {
            RoleID = -1;
            ExplorationItemDict = "{}";
            UnLockDict = "{}";
        }
    }
}
