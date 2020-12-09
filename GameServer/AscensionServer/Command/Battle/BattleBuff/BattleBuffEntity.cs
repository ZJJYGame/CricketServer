using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffEntity : IReference
    {
        //buffId
        int buffId;
        //拥有者Id
        int ownerId;
        //来源技能id
        int sourceSkillId;
        //buff持续时间
        int remainTime;
        //是否无限时间
        bool isInfiniteTime;
        int buffValue;
        BattleBuffEffectProperty battleBuffEffectProperty;
        //临界值
        int limitValue;
        bool isUp;
        //buff是否触发过
        bool hasTrigger;

        BattleBuffController battleBuffController;
        
        public void InitData(BattleSkillAddBuff battleSkillAddBuff,BattleBuffController owner,int skillId)
        {
            battleBuffController = owner;
            buffId = battleSkillAddBuff.BuffId;
            sourceSkillId = skillId;
            buffValue = battleSkillAddBuff.BuffValue;
            if (battleSkillAddBuff.DurationTime > 0)
            {
                remainTime = battleSkillAddBuff.DurationTime;
                isInfiniteTime = false;
            }
            else
            {
                isInfiniteTime = true;
            }
            limitValue = battleSkillAddBuff.TriggerLimitValue;
            isUp = battleSkillAddBuff.IsUp;
            hasTrigger = false;

            BattleBuffData battleBuffData = GameManager.CustomeModule<BattleRoomManager>().BattleBuffDataDict[buffId];
            battleBuffEffectProperty = battleBuffData.buffEffectProperty;

            battleBuffController.BuffTriggerEvent += OnTrigger;
            battleBuffController.BuffTimeUpdateEvent += OnTImeUpdate;
        }

        public void OnTrigger(IRoleBattleData roleBattleData)
        {
            if (hasTrigger)
                return;
            int healthPercent = roleBattleData.Health * 100 / roleBattleData.MaxHealth;
            if (isUp)
            {
                if (healthPercent <= limitValue)
                    return;
            }
            else
            {
                if (healthPercent > limitValue)
                    return;
            }
            //修改属性
            battleBuffController.ChangeProperty(battleBuffEffectProperty, buffValue);
            hasTrigger = true;
        }

        void OnTImeUpdate(int changeTime)
        {
            if (isInfiniteTime)
                return;
            Utility.Debug.LogWarning("remainTime=>" + remainTime + ",changeTime" + changeTime);
            remainTime -= changeTime;
            if (remainTime <= 0)
            {
                Utility.Debug.LogWarning("buff时间到了");
                int tempId = Convert.ToInt32(buffId.ToString() + sourceSkillId.ToString());
                battleBuffController.RemoveBuff(tempId);
            }
        }

        /// <summary>
        /// buff消失时触发，移除buff增加的效果
        /// </summary>
        public void OnRemove()
        {
            //恢复属性
            if (hasTrigger)
                battleBuffController.ChangeProperty(battleBuffEffectProperty, -buffValue);
        }


        public void Clear()
        {
            buffId = 0;
            sourceSkillId = 0;
            remainTime = 0;
            battleBuffController.BuffTriggerEvent -= OnTrigger;
            battleBuffController.BuffTimeUpdateEvent -= OnTImeUpdate;
        }
    }
}
