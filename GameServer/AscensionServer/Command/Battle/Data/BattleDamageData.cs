using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BattleDamageData
    {
        public int skillId;
        //伤害列表
        public List<int> damageNumList;
        public List<int> returnDamageNumList;
        //伤害是否暴击
        public List<bool> isCritList;
        //伤害是否闪避
        public List<bool> isDodgeList;
        public List<BattleSkillAddBuff> battleSkillAddBuffList;
        public BattleDamageData()
        {
            damageNumList = new List<int>();
            returnDamageNumList = new List<int>();
            isCritList = new List<bool>();
            isDodgeList = new List<bool>();
            battleSkillAddBuffList = new List<BattleSkillAddBuff>();
        }
    }
}
