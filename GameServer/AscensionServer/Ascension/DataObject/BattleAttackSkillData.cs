using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class BattleAttackSkillData
    {
        public int skillId;
        public string name;
        public string describe;
        public int enduranceCost;
        public int triggerProb;
        public int enduranceCostChangeEachLevel;
        public int triggerProbChangeEachLevel;
        public BattleSkillDamageData battleSkillDamageData;
        public List<BattleSkillAddBuffData> battleSkillAddBuffDataList;
    }
    [Serializable]
    public class BattleSkillDamageData
    {
        public int fixedValue;
        public int percentValue;
        public int attackNumber;
        public int fixedValueChangeEachLevel;
        public int percentValueChangeEachLevel;
    }
    [Serializable]
    public class BattleSkillAddBuffData
    {
        public int buffId;
        public int buffValue;
        public int buffLimitValue;
        public bool isUp;
        public int durationTime;
        public int buffValueChangeEachLevel;
        public int durationTimeChangeEachLevel;
        public int buffLimitValueChangeEachLevel;
        public bool TargetIsSelf;
    }
}
