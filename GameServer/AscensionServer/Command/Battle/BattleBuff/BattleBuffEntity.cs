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
        
        public void InitData(BattleSkillAddBuff battleSkillAddBuff)
        {
            buffId = battleSkillAddBuff.BuffId;
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
        }

        /// <summary>
        /// buff消失时触发，移除buff增加的效果
        /// </summary>
        public void OnRemove()
        {

        }

        public void Clear()
        {
            buffId = 0;
            sourceSkillId = 0;
            remainTime = 0;
        }
    }
}
