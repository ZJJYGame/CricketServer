using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffController
    {
        Dictionary<int, BattleBuffEntity> buffEntityDict;

        public void AddBuff(BattleSkillAddBuff battleSkillAddBuff,int skillId)
        {
            int id = Convert.ToInt32(battleSkillAddBuff.BuffId.ToString() + skillId.ToString());
            BattleBuffEntity battleBuffEntity = GameManager.ReferencePoolManager.Spawn<BattleBuffEntity>();
            battleBuffEntity.InitData(battleSkillAddBuff);
            if (buffEntityDict.ContainsKey(id))
            {
                buffEntityDict[id].OnRemove();
                GameManager.ReferencePoolManager.Despawn(buffEntityDict[id]);
                buffEntityDict[id] = battleBuffEntity;
            }
            else
            {
                buffEntityDict.Add(id, battleBuffEntity);
            }
        }

        public void RemoveBuff(int id)
        {
            
        }
    }
}
