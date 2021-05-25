using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class TowerFloorData
    {
        public int FloorId { get; set; }
        public int CricketId { get; set; }
        public List<int> RewardIdArray { get; set; }
        public List<int> RewardCountArray { get; set; }
    }
}
