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

        /// <summary>
        /// 随机使用一个技能
        /// </summary>
        public BattleSkill RandomSkill()
        {
            Random random = new Random();
            int randomNum = random.Next(0, roleBattleData.AllSkillProp);
            BattleSkill resultSkill;
            for (int i = 0; i < roleBattleData.BattleSkillList.Count; i++)
            {
                if (randomNum <= roleBattleData.BattleSkillList[i].TriggerProb)
                {
                    resultSkill = roleBattleData.BattleSkillList[i];
                    return resultSkill;
                }
            }
            return null;
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
