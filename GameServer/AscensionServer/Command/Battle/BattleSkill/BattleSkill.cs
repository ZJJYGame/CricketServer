using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BattleSkill
    {
        public int SkillId { get; private set; }
        public int EnduranceCost { get; private set; }
        public int TriggerProb { get; private set; }
        public int DamageFixedValue { get; private set; }
        public int DamagePercentValue { get; private set; }
        public int AttackNumber { get; private set; }
        public List<BattleSkillAddBuff> BattleSkillAddBuffList { get; private set; }

        public BattleSkill(BattleAttackSkillData battleAttackSkillData,int skillLevel)
        {
            SkillId = battleAttackSkillData.skillId;
            EnduranceCost = battleAttackSkillData.enduranceCost+battleAttackSkillData.enduranceCostChangeEachLevel*skillLevel;
            TriggerProb=battleAttackSkillData.triggerProb+ battleAttackSkillData.triggerProbChangeEachLevel * skillLevel;
            DamageFixedValue = battleAttackSkillData.battleSkillDamageData.fixedValue + battleAttackSkillData.battleSkillDamageData.fixedValueChangeEachLevel * skillLevel;
            DamagePercentValue = battleAttackSkillData.battleSkillDamageData.percentValue + battleAttackSkillData.battleSkillDamageData.percentValueChangeEachLevel * skillLevel;
            AttackNumber = battleAttackSkillData.battleSkillDamageData.attackNumber;
            BattleSkillAddBuffList = new List<BattleSkillAddBuff>();
            for (int i = 0; i < battleAttackSkillData.battleSkillAddBuffDataList.Count; i++)
            {
                BattleSkillAddBuffList.Add(new BattleSkillAddBuff(battleAttackSkillData.battleSkillAddBuffDataList[i],SkillId));
            }
        }
    }
    public class BattleSkillAddBuff
    {
        public int BuffId { get; private set; }
        public int BuffValue { get; private set; }
        public int DurationTime { get; private set; }
        public bool TargetSelf { get; private set; }

        public BattleSkillAddBuff(BattleSkillAddBuffData battleSkillAddBuffData,int skillLevel)
        {
            BuffId = battleSkillAddBuffData.buffId;
            BuffValue = battleSkillAddBuffData.buffValue + battleSkillAddBuffData.buffValueChangeEachLevel * skillLevel;
            DurationTime = battleSkillAddBuffData.durationTime + battleSkillAddBuffData.durationTimeChangeEachLevel * skillLevel;
            TargetSelf = battleSkillAddBuffData.TargetIsSelf;
        }
    }
}
