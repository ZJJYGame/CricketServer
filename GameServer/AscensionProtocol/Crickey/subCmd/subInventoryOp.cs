using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum subInventoryOp:byte
    {
        None = 0,
        Get = 1,
        Add = 2,
        Update = 3,
        Remove = 4,
        Verify = 5
    }
}
