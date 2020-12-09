using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class CricketLevel
    {
        public int Level { set; get; }
        public int AssignPoint { set; get; }
        public int ExpUP { set; get; }
    }
}
