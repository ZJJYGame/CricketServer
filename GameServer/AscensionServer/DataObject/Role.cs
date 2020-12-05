using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class Role
    {
        public virtual int RoleID { get; set; }
        public virtual int RoleName { get; set; }
    }
}
