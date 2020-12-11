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

        public BattleBuffController battleBuffController;

        public void Init(int roleId)
        {
            this.RoleId = roleId;
            battleBuffController = new BattleBuffController(roleBattleData);
            roleBattleData = GetRoleBattleData(roleId);
            RemainActionBar = roleBattleData.ActionBar;
            battleBuffController.roleBattleData = roleBattleData;
        }

        /// <summary>
        /// 随机使用一个技能
        /// </summary>
        public BattleSkill RandomSkill(bool isAttackSkill)
        {
            List<BattleSkill> randomSkillList;
            int randomNum;
            if (isAttackSkill)
            {
                randomSkillList = roleBattleData.BattleAttackSkillList;
                randomNum = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, roleBattleData.AllAttackSkillProp);
            }
            else
            {
                randomSkillList = roleBattleData.BattleDefendSkillList;
                randomNum = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, roleBattleData.AllDefendSkillProp);
            }
            
            BattleSkill resultSkill;
            int propNum=0;
            for (int i = 0; i < randomSkillList.Count; i++)
            {
                propNum += randomSkillList[i].TriggerProb;
                if (randomNum <= propNum)
                {
                    resultSkill = randomSkillList[i];
                    if (roleBattleData.Endurance > 0)
                        return resultSkill;
                    else
                        return null;
                }
            }
            Utility.Debug.LogInfo("没有符合的技能");
            return null;
        }

        public void ChangeActionBar(int num)
        {
            RemainActionBar -= num;

        }
        public void TryRestartActionBar()
        {
                RemainActionBar = roleBattleData.ActionBar;

        }

        //待完善，需要从数据库拿取人物数据
        RoleBattleData GetRoleBattleData(int roleId)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController) { };
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
