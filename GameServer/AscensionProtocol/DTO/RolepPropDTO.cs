using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class RolepPropDTO
    {
        public virtual int RoleID { set; get; }
        public virtual int PropID { set; get; }
        public virtual int PropNum { set; get; }
        public virtual int CricketID { set; get; }
        public virtual int PropType { set; get; }
    }
}
