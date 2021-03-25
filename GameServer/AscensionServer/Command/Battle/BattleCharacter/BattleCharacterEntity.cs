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
        public bool IsWin { get;set; }
        //行动次数记录
        public int ActionCount { get; set; }

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
            IsWin = true;
            ActionCount = 1;
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
            IsWin = true;
            ActionCount = 1;
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
            IsRobot = true;
            IsWin = true;
            ActionCount = 1;
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
                Utility.Debug.LogError("随机技能数量" + randomSkillList.Count);
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
            Utility.Debug.LogInfo(CricketID+"没有符合的技能");
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
        public void AddActionCount()
        {
            ActionCount++;
        }

        RoleBattleData GetRoleBattleData(int roleId)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController,this) { };
            return roleBattleData;
        }
        RoleBattleData GetRoleBattleData(RoleDTO roleDTO,CricketDTO cricketDTO)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController, roleDTO, cricketDTO,this) { };
            return roleBattleData;
        }
        RoleBattleData GetRoleBattleData(MachineData machineData)
        {
            RoleBattleData roleBattleData = new RoleBattleData(battleBuffController,machineData,this) { };
            return roleBattleData;
        }

        /// <summary>
        /// 使用被动技能
        /// </summary>
        public void UsePassiveSkill()
        {
            List<BattleSkill> battlePassiveSkillList = roleBattleData.BattlePassiveSkillList;
            BattleSkill battlePassiveSkill;
            for (int i = 0; i < battlePassiveSkillList.Count; i++)
            {
                battlePassiveSkill = battlePassiveSkillList[i];
                for (int j = 0; j < battlePassiveSkill.BattleSkillAddBuffList.Count; j++)
                {
                    Utility.Debug.LogError(CricketID+"添加被动技能" + battlePassiveSkill.SkillId);
                    battleBuffController.AddBuff(battlePassiveSkill.BattleSkillAddBuffList[j], battlePassiveSkill.SkillId);
                }
            }
            battleBuffController.TriggerBuff();
        }

        public void S2CSendBattleData(BattleTransferDTO battleTransferDTO)
        {
            if (IsRobot)
                return;
            GameManager.CustomeModule<BattleRoomManager>().S2CEnterBattle(RoleID, battleTransferDTO);
        }

        public void Clear()
        {
            RoleID = 0;
            RoleName = null;
            CricketID = 0;
            RemainActionBar = 0;
            IsRobot = false;
            roleBattleData = null;
            battleBuffController = null;
            ActionCount = 0;
        }

        public void OnRefresh()
        {
        }
    }
}
