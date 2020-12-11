using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class NestLevel
    {
        public int NestID { set; get; }
        public int Gold { set; get; }
    }
}
