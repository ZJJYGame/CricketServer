using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class TowerAwardData
    {
        public int RewardID;
        public List<int> GoodsID;
        public List<int> Weight;
    }
}
