using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    [CustomeModule]
    public class BattleCharacterManager:Module<BattleCharacterManager>
    {
        Dictionary<int, BattleCharacterEntity> battleCharacterEntityDict = new Dictionary<int, BattleCharacterEntity>();

        public BattleCharacterEntity CreateCharacter(int roleId)
        {
            BattleCharacterEntity battleCharacterEntity = GameManager.ReferencePoolManager.Spawn<BattleCharacterEntity>();
            battleCharacterEntity.Init(roleId);
            if (battleCharacterEntityDict.ContainsKey(roleId))
            {
                GameManager.ReferencePoolManager.Despawn(battleCharacterEntityDict[roleId]);
                battleCharacterEntityDict.Remove(roleId);
            }
            battleCharacterEntityDict.Add(roleId, battleCharacterEntity);
            return battleCharacterEntity;
        }
        public void RemoveCharacter(int roleId)
        {
            if (battleCharacterEntityDict.ContainsKey(roleId))
            {
                GameManager.ReferencePoolManager.Despawn(battleCharacterEntityDict[roleId]);
                battleCharacterEntityDict.Remove(roleId);
            }
        }
    }
}
