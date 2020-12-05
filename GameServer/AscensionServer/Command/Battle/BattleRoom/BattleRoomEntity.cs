using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Reference;

namespace AscensionServer
{
    public class BattleRoomEntity : IReference
    {
        int roomId;
        BattleCharacterEntity battleCharacterEntity_one;
        BattleCharacterEntity battleCharacterEntity_Two;

        public void Init(int roomId,int playerOneId,int playerTwoId)
        {
            this.roomId = roomId;
            battleCharacterEntity_one = GameManager.CustomeModule<ReferencePoolManager>().Spawn<BattleCharacterEntity>();
            battleCharacterEntity_one.Init(playerOneId);
            battleCharacterEntity_Two = GameManager.CustomeModule<ReferencePoolManager>().Spawn<BattleCharacterEntity>();
            battleCharacterEntity_Two.Init(playerTwoId);
        }


        public void Clear()
        {
            roomId = 0;
            GameManager.CustomeModule<ReferencePoolManager>().Despawn(battleCharacterEntity_one);
            GameManager.CustomeModule<ReferencePoolManager>().Despawn(battleCharacterEntity_Two);
            battleCharacterEntity_one = null;
            battleCharacterEntity_Two = null;
        }

        public void OnRefresh()
        {
        }
    }
}
