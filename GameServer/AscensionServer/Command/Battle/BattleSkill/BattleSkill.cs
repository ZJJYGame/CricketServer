using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

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
            EnduranceCost = battleAttackSkillData.enduranceCost+battleAttackSkillData.enduranceCostChangeEachLevel*(skillLevel-1);
            TriggerProb=battleAttackSkillData.triggerProb+ battleAttackSkillData.triggerProbChangeEachLevel * (skillLevel - 1);
            DamageFixedValue = battleAttackSkillData.battleSkillDamageData.fixedValue + battleAttackSkillData.battleSkillDamageData.fixedValueChangeEachLevel * (skillLevel - 1);
            DamagePercentValue = battleAttackSkillData.battleSkillDamageData.percentValue + battleAttackSkillData.battleSkillDamageData.percentValueChangeEachLevel * (skillLevel - 1);
            AttackNumber = battleAttackSkillData.battleSkillDamageData.attackNumber;
            BattleSkillAddBuffList = new List<BattleSkillAddBuff>();
            for (int i = 0; i < battleAttackSkillData.battleSkillAddBuffDataList.Count; i++)
            {
                BattleSkillAddBuffList.Add(new BattleSkillAddBuff(battleAttackSkillData.battleSkillAddBuffDataList[i], skillLevel));
            }
        }
    }
    public class BattleSkillAddBuff
    {
        public int BuffId { get; private set; }
        public int BuffValue { get; private set; }
        public int DurationTime { get; private set; }
        //触发临界值
        public int TriggerLimitValue { get; private set; }
        public bool IsUp { get; private set; }
        public bool TargetSelf { get; private set; }

        public BattleSkillAddBuff(BattleSkillAddBuffData battleSkillAddBuffData,int skillLevel)
        {
            BuffId = battleSkillAddBuffData.buffId;
            BuffValue = battleSkillAddBuffData.buffValue + battleSkillAddBuffData.buffValueChangeEachLevel * (skillLevel - 1);
            DurationTime = battleSkillAddBuffData.durationTime + battleSkillAddBuffData.durationTimeChangeEachLevel * (skillLevel - 1);
            TargetSelf = battleSkillAddBuffData.TargetIsSelf;
            TriggerLimitValue = battleSkillAddBuffData.buffLimitValue + battleSkillAddBuffData.buffLimitValueChangeEachLevel * (skillLevel - 1);
            IsUp = battleSkillAddBuffData.isUp;
        }
    }
}
