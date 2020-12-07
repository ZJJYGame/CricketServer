using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class RoleAssets: IReference
    {
        public virtual int RoleID { get; set; }
        public virtual int RoleGold { get; set; }

        public RoleAssets()
        {
            RoleGold = 5000;
        }
        public void Clear()
        {
            
        }
    }
}
