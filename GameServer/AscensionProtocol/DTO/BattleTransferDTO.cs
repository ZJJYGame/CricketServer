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
        public List<int> DamageList { get; set; }
        public List<bool> IsCritList { get; set; }
        public List<bool> IsDodgeList { get; set; }
        public List<int> ReturnDamageList { get; set; }
        public List<int> AddBuffList { get; set; }
        public BattleActionData()
        {
            DamageList = new List<int>();
            IsCritList = new List<bool>();
            IsDodgeList = new List<bool>();
            ReturnDamageList = new List<int>();
            AddBuffList = new List<int>();
        }
    }

}
