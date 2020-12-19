using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
   public class ADAward
    {
        public int PropID { set; get; }
        public int Probability { set; get; }
        public int PropType { set; get; }
        public List<int> AddNumber { set; get; }
    }
}
