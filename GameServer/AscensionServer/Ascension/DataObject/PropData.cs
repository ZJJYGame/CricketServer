using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class PropData
    {
        public int PropID { get; set; }
        public string PropName { get; set; }
        public int PropType { get; set; }
        public int AddNumber { get; set; }
        public int SkillID { get; set; }
    }
}
