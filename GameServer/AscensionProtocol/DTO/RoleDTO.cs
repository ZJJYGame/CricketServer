using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionProtocol
{
    [Serializable]
    public class RoleDTO
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleName { get; set; }

    }
    
}
