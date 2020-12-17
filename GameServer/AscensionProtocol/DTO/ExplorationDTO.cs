using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class ExplorationDTO
    {
        public int RoleID { get; set; }

        public Dictionary<int, ExplorationItemDTO> ExplorationItemDict { get; set; }
    }
    [Serializable]
    public class ExplorationItemDTO
    {
        public List<int> ItemId { get; set; }
        public DateTime DateTime { get; set; }
        public int TimeType { get; set; }
        public int CustomId { get; set; }
        public int GlobalId { get; set; }
    }
}
