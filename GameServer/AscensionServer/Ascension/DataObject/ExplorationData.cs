using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class ExplorationData
    {
        public int EventID { get; set; }
        public string EventDescribe { get; set; }
        public List<int> Probability { get; set; }
        public string EventType { get; set; }
        public List<int> PropID { get; set; }
        public int SkillID { get; set; }
        public List<int> Number { get; set; }
        public List<int> GradeRange { get; set; }
        public int EventRace { get; set; }
        public List<int> EventA { get; set; }
        public List<int> EventB { get; set; }
        public List<int> EventC { get; set; }
    }
}
