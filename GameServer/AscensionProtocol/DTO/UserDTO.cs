using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class UserDTO
    {
        public virtual string Account { get; set; }
        public virtual string Password { get; set; }
        public virtual string UUID { get; set; }
    }
}