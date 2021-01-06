using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;

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
        public BattleCharacterEntity CreateCharacter(RoleDTO roleDTO,CricketDTO cricketDTO)
        {
            BattleCharacterEntity battleCharacterEntity = GameManager.ReferencePoolManager.Spawn<BattleCharacterEntity>();
            battleCharacterEntity.Init(roleDTO, cricketDTO);
            if (battleCharacterEntityDict.ContainsKey(battleCharacterEntity.RoleID))
            {
                GameManager.ReferencePoolManager.Despawn(battleCharacterEntityDict[battleCharacterEntity.RoleID]);
                battleCharacterEntityDict.Remove(battleCharacterEntity.RoleID);
            }
            battleCharacterEntityDict.Add(battleCharacterEntity.RoleID, battleCharacterEntity);
            return battleCharacterEntity;
        }
        public BattleCharacterEntity CreateCharacter(RoleDTO roleDTO, CricketDTO cricketDTO,MachineData machineData)
        {
            BattleCharacterEntity battleCharacterEntity = GameManager.ReferencePoolManager.Spawn<BattleCharacterEntity>();
            battleCharacterEntity.Init(roleDTO, cricketDTO,machineData);
            if (battleCharacterEntityDict.ContainsKey(battleCharacterEntity.RoleID))
            {
                GameManager.ReferencePoolManager.Despawn(battleCharacterEntityDict[battleCharacterEntity.RoleID]);
                battleCharacterEntityDict.Remove(battleCharacterEntity.RoleID);
            }
            battleCharacterEntityDict.Add(battleCharacterEntity.RoleID, battleCharacterEntity);
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
