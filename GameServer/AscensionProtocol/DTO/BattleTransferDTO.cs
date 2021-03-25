using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class BattleTransferDTO
    {
        public BattleRoleData RoleOneData { get; set; }
        public BattleRoleData RoleTwoData { get; set; }
        public List<BattleRoleActionData> BattleRoleActionDataList { get; set; }
    }

    public class BattleRoleData
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int HeadIconID { get; set; }
        public int CricketId { get; set; }
        public int AssetID { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int MaxEndurance { get; set; }
        public int Endurance { get; set; }
        public int ActionBar { get; set; }
        public List<TriggerSkillData> PassiveSkill { get; set; }
        public BattleResult BattleResult { get; set; }
    }
    [Serializable]
    public class BattleRoleActionData
    {
        public int Time { get; set; }
        public bool IsCrash { get; set; }
        public BattleActionData AttackBattleActionData { get; set; }
        public BattleActionData DefendBattleActionData { get; set; }
        public BattleRoleActionData()
        {
            AttackBattleActionData = new BattleActionData();
            DefendBattleActionData = new BattleActionData();
        }
    }
    [Serializable]
    public class BattleActionData
    {
        public int RoleId { get; set; }
        public int SkillId { get; set; }
        public int EndurenceReply { get; set; }
        public List<int> DamageList { get; set; }
        public List<bool> IsCritList { get; set; }
        public List<bool> IsDodgeList { get; set; }
        public List<int> ReturnDamageList { get; set; }
        public List<int> AddBuffList { get; set; }
        public List<int> AddBuffDurationTime { get; set; }
        public List<TriggerSkillData> TriggerSkillList { get; set; }


        public BattleActionData()
        {
            DamageList = new List<int>();
            IsCritList = new List<bool>();
            IsDodgeList = new List<bool>();
            ReturnDamageList = new List<int>();
            AddBuffList = new List<int>();
            AddBuffDurationTime = new List<int>();
            TriggerSkillList = new List<TriggerSkillData>();
        }
    }

    public class TriggerSkillData
    {
        public int SkillId { get; set; }
        public int EnduranceCost { get; set; }
        public List<int> AddBuffId { get; set; }
        public List<int> DurationTime { get; set; }

        public TriggerSkillData()
        {
            AddBuffId = new List<int>();
            DurationTime = new List<int>();
           
        }
    }
    //战斗结果
    public class BattleResult
    {
        public int GetMoney { get; set; }
        public int GetExp { get; set; }
        public int RankLevel { get; set; }
        public bool IsWinner { get; set; }
    }
}
