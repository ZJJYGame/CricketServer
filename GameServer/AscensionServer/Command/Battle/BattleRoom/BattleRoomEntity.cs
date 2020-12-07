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

        BattleController battleController;

        public void Init(int roomId,int playerOneId,int playerTwoId)
        {
            this.roomId = roomId;
            battleCharacterEntity_one = GameManager.CustomeModule<BattleCharacterManager>().CreateCharacter(playerOneId);
            battleCharacterEntity_Two = GameManager.CustomeModule<BattleCharacterManager>().CreateCharacter(playerTwoId);
            battleController = new BattleController();
            battleController.InitController(battleCharacterEntity_one, battleCharacterEntity_Two);
            battleController.StartBattle();
        }


        public void Clear()
        {
            roomId = 0;
            GameManager.CustomeModule<BattleCharacterManager>().RemoveCharacter(battleCharacterEntity_one.RoleId);
            GameManager.CustomeModule<BattleCharacterManager>().RemoveCharacter(battleCharacterEntity_Two.RoleId);
            battleCharacterEntity_one = null;
            battleCharacterEntity_Two = null;
        }

        public void OnRefresh()
        {
        }
    }
}
