using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class RoleBattleData: IRoleBattleData
    {
        //攻击力
        int attack;
        public int Attack { get { return attack + buffRoleBattleData.Attack; } }
        //最大血量
        int maxHealth;
        public int MaxHealth { get { return maxHealth + buffRoleBattleData.MaxHealth; }  }
        //血量
        int health;
        public int Health { get { return health + buffRoleBattleData.Health; } }
        //防御力
        int defence;
        public int Defence { get { return defence + buffRoleBattleData.Defence; } }
        //最大耐力
        int maxEndurance;
        public int MaxEndurance { get { return maxEndurance + buffRoleBattleData.MaxEndurance; }  }
        //耐力
        int endurance;
        public int Endurance { get { return endurance + buffRoleBattleData.Endurance; } }
        //耐力回复
        int enduranceReply;
        public int EnduranceReply { get { return enduranceReply + buffRoleBattleData.EnduranceReply; }  }
        //行动条
        int actionBar;
        public int ActionBar { get { return actionBar;}  }
        //暴击率
        int critProp;
        public int CritProp
        {
            get
            {
                int num = critProp + buffRoleBattleData.CritProp;
                return num > 80 ? 80 : num;
            }
        }
        //闪避率
        int dodgeProp;
        public int DodgeProp { get { return dodgeProp + buffRoleBattleData.DodgeProp; } }
        //受到伤害
        int receiveDamage;
        public int ReceiveDamage
        {
            get
            {
                int num = receiveDamage + buffRoleBattleData.ReceiveDamage;
                return num < 20 ? 20 : num;
            }
        }
        //穿透
        int pierce;
        public int Pierce
        {
            get
            {
                int num = pierce + buffRoleBattleData.Pierce;
                return num > 50 ? 50 : num;
            }
        }
        //反伤
        int reboundDamage;
        public int ReboundDamage
        {
            get
            {
                int num = reboundDamage + buffRoleBattleData.ReboundDamage;
                return num > 50 ? 50 : num;
            }
        }
        //暴击伤害
        int critDamage;
        public int CritDamage { get { return critDamage + buffRoleBattleData.CritDamage; }  }
        //暴击抗性
        int critResistance;
        public int CritResistance
        {
            get
            {
                int num = critResistance + buffRoleBattleData.CritResistance;
                return num > 80 ? 80 : num;
            }
        }

        //玩家攻击技能列表
        public List<BattleSkill> BattleAttackSkillList { get; private set; }
        //所有攻击技能总概率
        public int AllAttackSkillProp { get; private set; }

        //玩家受击技能列表
        public List<BattleSkill> BattleDefendSkillList { get; private set; }
        //所有受击技能总概率
        public int AllDefendSkillProp { get; private set; }

        IRoleBattleData buffRoleBattleData;

        /// <summary>
        /// 受到的直接伤害
        /// </summary>
        public void OnHurt(BattleDamageData battleDamageData)
        {
            for (int i = 0; i < battleDamageData.damageNumList.Count; i++)
            {
                health -= battleDamageData.damageNumList[i];
            }
        }
        /// <summary>
        /// 受到的反伤伤害
        /// </summary>
        public void OnReboundHurt(BattleDamageData battleDamageData)
        {
            for (int i = 0; i < battleDamageData.returnDamageNumList.Count; i++)
            {
                health -= battleDamageData.returnDamageNumList[i];
            }
        }
        /// <summary>
        /// 耐力消耗
        /// </summary>
        public void OnEnduranceCost(BattleSkill battleSkill)
        {
            if (battleSkill == null)
                return;
            endurance -= battleSkill.EnduranceCost;
            endurance = endurance < 0 ? 0 : endurance;
        }

        public void OnEnduranceReply()
        {
            endurance += enduranceReply;
        }

        public RoleBattleData(IRoleBattleData buffRoleBattleData)
        {
            attack = 20;
            maxHealth = 500;
            health = 500;
            defence =5;
            maxEndurance = 200;
            endurance = 200;
            enduranceReply = 20;
            actionBar = GameManager.CustomeModule<BattleRoomManager>().random.Next(6, 11)*100;
            critProp = 25;
            dodgeProp = 25;
            receiveDamage = 100;
            pierce = 10;
            reboundDamage = 10;
            critDamage = 150;
            critResistance = 0;
            this.buffRoleBattleData = buffRoleBattleData;
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleAttackSkillData>>(out var tempSkillDict);
            BattleAttackSkillList = new List<BattleSkill>();
            BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[3001],1));
            BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[3002],1));
            BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[3003],1));
            BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[3004],1));
            for (int i = 0; i < BattleAttackSkillList.Count; i++)
            {
                AllAttackSkillProp += BattleAttackSkillList[i].TriggerProb;
            }

            BattleDefendSkillList = new List<BattleSkill>();
            BattleDefendSkillList.Add(new BattleSkill(tempSkillDict[3010], 1));
            BattleDefendSkillList.Add(new BattleSkill(tempSkillDict[3011], 1));
            BattleDefendSkillList.Add(new BattleSkill(tempSkillDict[3017], 1));
            for (int i = 0; i < BattleDefendSkillList.Count; i++)
            {
                AllDefendSkillProp += BattleDefendSkillList[i].TriggerProb;
            }
        }
    }
}
