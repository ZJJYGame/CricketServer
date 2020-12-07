using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class RoleBattleData
    {
        //攻击力
        public int Attack { get; private set; }
        //最大血量
        public int MaxHealth { get; private set; }
        //血量
        public int Health { get; private set; }
        //防御力
        public int Defence { get; private set; }
        //最大耐力
        public int MaxEndurance { get; private set; }
        //耐力
        public int Endurance { get; private set; }
        //耐力回复
        public int EnduranceReply { get; private set; }
        //行动条
        public int ActionBar { get; private set; }
        //暴击率
        public int CritProp { get; private set; }
        //闪避率
        public int DodgeProp { get; private set; }
        //受到伤害
        public int ReceiveDamage { get; private set; }
        //穿透
        public int Pierce { get; private set; }
        //反伤
        public int ReboundDamage { get; private set; }
        //暴击伤害
        public int CritDamage { get; private set; }
        //暴击抗性
        public int CritResistance { get; private set; }

        //玩家战斗技能列表
        public List<BattleSkill> BattleSkillList { get; private set; }
        //所有技能总概率
        public int AllSkillProp { get; private set; }
        
        /// <summary>
        /// 受到的直接伤害
        /// </summary>
        public void OnHurt(BattleDamageData battleDamageData)
        {
            for (int i = 0; i < battleDamageData.damageNumList.Count; i++)
            {
                Health -= battleDamageData.damageNumList[i];
            }
        }
        /// <summary>
        /// 受到的反伤伤害
        /// </summary>
        public void OnReboundHurt(BattleDamageData battleDamageData)
        {
            for (int i = 0; i < battleDamageData.returnDamageNumList.Count; i++)
            {
                Health -= battleDamageData.returnDamageNumList[i];
            }
        }
        /// <summary>
        /// 耐力消耗
        /// </summary>
        public void OnEnduranceCost(BattleSkill battleSkill)
        {
            Endurance -= battleSkill.EnduranceCost;
        }

        public RoleBattleData()
        {
            Attack = 20;
            MaxHealth = 100;
            Health = 100;
            Defence =5;
            MaxEndurance = 200;
            Endurance = 200;
            EnduranceReply = 20;
            ActionBar = 1000;
            CritProp = 25;
            DodgeProp = 25;
            ReceiveDamage = 100;
            Pierce = 10;
            ReboundDamage = 10;
            CritDamage = 150;
            CritResistance = 0;
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleAttackSkillData>>(out var tempSkillDict);
            BattleSkillList = new List<BattleSkill>();
            BattleSkillList.Add(new BattleSkill(tempSkillDict[3001],1));
            BattleSkillList.Add(new BattleSkill(tempSkillDict[3002],1));
            BattleSkillList.Add(new BattleSkill(tempSkillDict[3003],1));
            BattleSkillList.Add(new BattleSkill(tempSkillDict[3004],1));
            for (int i = 0; i < BattleSkillList.Count; i++)
            {
                AllSkillProp += BattleSkillList[i].TriggerProb;
            }
        }
    }
}
