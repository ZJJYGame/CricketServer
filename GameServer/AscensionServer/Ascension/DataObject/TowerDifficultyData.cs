using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class TowerDifficultyData
    {
        public int DifficultyId { get; set; }
        public string Name { get; set; }
        public string IconPath { get; set; }
        public List<int> FloorIdArray { get; set; }
    }
}
