using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class xRTask
    {
        public virtual int RoleID { get; set; }
        public virtual string taskDict { get; set; }

        public xRTask()
        {
            RoleID = -1;
            taskDict = "{}";
        }
    }
}
