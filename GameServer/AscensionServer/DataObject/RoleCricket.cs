using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class RoleCricket
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleCrickets { get; set; }
        public RoleCricket()
        {
            RoleCrickets = "[]";
        }
    }
}
