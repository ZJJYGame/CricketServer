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
        public int RoleId { get; private set; }
        public int RemainActionBar { get; private set; }

        public RoleBattleData roleBattleData;



        public void Init(int roleId)
        {
            this.RoleId = roleId;
            roleBattleData = GetRoleBattleData(roleId);
            RemainActionBar = roleBattleData.ActionBar;
        }

        /// <summary>
        /// 随机使用一个技能
        /// </summary>
        public BattleSkill RandomSkill()
        {
            int randomNum = GameManager.CustomeModule<BattleRoomManager>() .random.Next(0, roleBattleData.AllSkillProp);
            BattleSkill resultSkill;
            int propNum=0;
            for (int i = 0; i < roleBattleData.BattleSkillList.Count; i++)
            {
                propNum += roleBattleData.BattleSkillList[i].TriggerProb;
                if (randomNum <= propNum)
                {
                    resultSkill = roleBattleData.BattleSkillList[i];
                    return resultSkill;
                }
            }
            Utility.Debug.LogInfo("没有符合的技能");
            return null;
        }

        public void ChangeActionBar(int num)
        {
            RemainActionBar -= num;
            if (RemainActionBar <= 0)
            {
                RemainActionBar = roleBattleData.ActionBar;
            }
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
