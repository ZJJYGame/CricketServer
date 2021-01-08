using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class BattlePassiveSkillData
    {
        public int skillId;
        public string name;
        public bool isAttackSkill;
        public string describe;
        public int enduranceCost;
        public int triggerProb;
        public int enduranceCostChangeEachLevel;
        public int triggerProbChangeEachLevel;
        public BattleSkillDamageData battleSkillDamageData;
        public List<BattleSkillAddBuffData> battleSkillAddBuffDataList;
    }
}
