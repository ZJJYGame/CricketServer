using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class Shop
    {
        public int PropID { get; set; }
        public int PropPrice { get; set; }
        public int PropType { get; set; }
    }
}
