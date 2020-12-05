using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleCharacterEntity : IReference
    {
        int roleId;
        public int remainActionBar;

        public RoleBattleData roleBattleData;

        public void Init(int roleId)
        {
            this.roleId = roleId;
            roleBattleData = GetRoleBattleData(roleId);
            remainActionBar = roleBattleData.ActionBar;
        }

        //待完善，需要从数据库拿取人物数据
        RoleBattleData GetRoleBattleData(int roleId)
        {
            RoleBattleData roleBattleData = new RoleBattleData() { };
            return roleBattleData;
        }

        public void Clear()
        {
        }

        public void OnRefresh()
        {
        }
    }
}
