using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class RoleAssetsDTO
    {
        public virtual int RoleID { get; set; }
        public virtual int RoleGold { get; set; }
    }
}
