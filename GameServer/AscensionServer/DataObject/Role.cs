using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class Role: IReference
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleName { get; set; }

        public Role()
        {
            RoleName = "蛐蛐你个大蝈蝈";
        }
        public void Clear()
        {
    
        }
    }
}
