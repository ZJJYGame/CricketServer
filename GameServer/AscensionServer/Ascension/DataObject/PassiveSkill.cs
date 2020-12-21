using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class PassiveSkill
    {
        public int SkillID { get; set; }
        public List<int> Attribute { get; set; }//加成属性点
        public List<int> Percentage { get; set; }//加成百分比
        public List<int> Fixed { get; set; }//固定数值
        public List<int> LevelPercentage { get; set; }//等级影响百分比
        public List<int> LevelFixed { get; set; }//等级影响固定数值
    }
}
