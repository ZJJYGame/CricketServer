using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class RankLevel
    {
        public int RankID { get; set; }
        public string RankName { get; set; }
        public int NextID { get; set; }
        public int UpperID { get; set; }
        public string RankIcon { get; set; }
    }
}
