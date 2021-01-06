using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;

namespace AscensionServer
{
    public class BattleCharacterEntity : IReference
    {
        public int RoleID { get; private set; }
        public string RoleName { get; private set; }
        public int CricketID { get; private set; }
        public int RemainActionBar { get; private set; }
        public bool IsRobot { get; private set; }

        public RoleBattleData roleBattleData;

        public BattleBuffController battleBuffController;

        public void Init(int roleId)
        {
            this.CricketID = roleId;
            battleBuffController = new BattleBuffController(roleBattleData);
            roleBattleData = GetRoleBattleData(roleId);
            RemainActionBar = roleBattleData.ActionBar;
            battleBuffController.roleBattleData = roleBattleData;
            IsRobot = false;
        }
        public void Init(RoleDTO roleDTO,CricketDTO cricketDTO)
        {
            RoleID = roleDTO.RoleID;
            RoleName = roleDTO.RoleName;
            CricketID = cricketDTO.ID;
            battleBuffController = new BattleBuffController(roleBattleData);
            roleBattleData = GetRoleBattleData(roleDTO, cricketDTO);
            RemainActionBar = roleBattleData.ActionBar;
            battleBuffController.roleBattleData = roleBattleData;
            IsRobot = false;
        }
        //机器人
        public void Init(RoleDTO roleDTO, CricketDTO cricketDTO,MachineData machineData)
        {
            //todo机器人RoleID
            RoleName = machineData.UserName;
            //todo蛐蛐唯一ID
            battleBuffController = new BattleBuffController(roleBattleData);
            roleBattleData = GetRoleBattleData(machineData);
            RemainActionBar = roleBattleData.ActionBar;
            battleBuffController.roleBattleData = roleBattleData;
            IsRobot = false;
        }
        /// <summary>
        /// 随机使用一个技能
        /// </summary>
        public BattleSkill RandomSkill(bool isAttackSkill)
        {
            if (roleBattleData.Endurance <= 0)
                return null;
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
                    return resultSkill;
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
        RoleBattleData GetRoleBattleData(RoleDTO roleDTO,CricketDTO cricketDTO)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController, roleDTO, cricketDTO) { };
            return roleBattleData;
        }
        RoleBattleData GetRoleBattleData(MachineData machineData)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController,machineData) { };
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
