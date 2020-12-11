using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class RoleShopDTO
    {
        public virtual int RoleID { set; get; }
        public virtual int PropID { set; get; }
        public virtual int PropNum { set; get; }
    }
}
