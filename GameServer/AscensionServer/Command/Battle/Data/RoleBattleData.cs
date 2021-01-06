﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;

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
            attack = 100;
            maxHealth = 1000;
            health = 1000;
            defence =100;
            maxEndurance = 500;
            endurance = 500;
            enduranceReply = 200;
            actionBar = 2000;
            critProp = 20;
            dodgeProp = 50;
            receiveDamage = 100;
            pierce = 30;
            reboundDamage = 10;
            critDamage = 50;
            critResistance = 0;
            this.buffRoleBattleData = buffRoleBattleData;
            Dictionary<int, BattleAttackSkillData> tempSkillDict = GameManager.CustomeModule<BattleRoomManager>().battleAttackSkillDataDict;
            List<BattleAttackSkillData> tempSkillList = tempSkillDict.Values.ToList();
            BattleAttackSkillList = new List<BattleSkill>();
            //for (int i = 0; i < tempSkillList.Count; i++)
            //{
            //    if (tempSkillList[i].isAttackSkill)
            //        BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[tempSkillList[i].skillId], 1));
            //}
            BattleAttackSkillList.Add(new BattleSkill(tempSkillDict[3005], 1));
            for (int i = 0; i < BattleAttackSkillList.Count; i++)
            {
                AllAttackSkillProp += BattleAttackSkillList[i].TriggerProb;
            }

            BattleDefendSkillList = new List<BattleSkill>();
            //for (int i = 0; i < tempSkillList.Count; i++)
            //{
            //    if (!tempSkillList[i].isAttackSkill)
            //        BattleDefendSkillList.Add(new BattleSkill(tempSkillDict[tempSkillList[i].skillId], 1));
            //}
            //BattleDefendSkillList.Add(new BattleSkill(tempSkillDict[3337], 1));
            for (int i = 0; i < BattleDefendSkillList.Count; i++)
            {
                AllDefendSkillProp += BattleDefendSkillList[i].TriggerProb;
            }
        }
        public RoleBattleData(IRoleBattleData buffRoleBattleData,RoleDTO roleDTO,CricketDTO cricketDTO)
        {
            Utility.Debug.LogError("战斗匹配蛐蛐ID"+Utility.Json.ToJson(cricketDTO));
            var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketDTO.ID);
            var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);
            attack = (int)cricketstatus.Atk;
            maxHealth = (int)cricketstatus.Hp;
            health = (int)cricketstatus.Hp;
            defence = (int)cricketstatus.Defense;
            maxEndurance = (int)cricketstatus.Mp;
            endurance = (int)cricketstatus.Mp;
            enduranceReply = (int)cricketstatus.MpReply;
            actionBar = (int)cricketstatus.Speed;
            critProp = (int)cricketstatus.Crt;
            dodgeProp = (int)cricketstatus.Eva;
            receiveDamage = 100 - (int)cricketstatus.ReduceAtk;
            pierce = (int)cricketstatus.ReduceDef;
            reboundDamage = (int)cricketstatus.Rebound;
            critDamage = (int)cricketstatus.CrtAtk-100;
            critResistance = (int)cricketstatus.CrtDef;
            this.buffRoleBattleData = buffRoleBattleData;
            Dictionary<int, BattleAttackSkillData> tempAttackSkillDict = GameManager.CustomeModule<BattleRoomManager>().battleAttackSkillDataDict;
            List<BattleAttackSkillData> tempAttackSkillList = tempAttackSkillDict.Values.ToList();

            BattleAttackSkillList = new List<BattleSkill>();
            BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[4000],1));
            BattleDefendSkillList = new List<BattleSkill>();

            var tempSkillIdList = cricketDTO.SkillDict.Values.ToList();
            var tempSpecialSkillIdList = cricketDTO.SpecialDict.Values.ToList();
            for (int i = 0; i < tempSkillIdList.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(tempSkillIdList[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[tempSkillIdList[i]];
                    if (battleAttackSkillData.isAttackSkill)
                        BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                    else
                        BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                }
            }
            for (int i = 0; i < tempSpecialSkillIdList.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(tempSpecialSkillIdList[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[tempSpecialSkillIdList[i]];
                    if (battleAttackSkillData.isAttackSkill)
                        BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSpecialSkillIdList[i]]));
                    else
                        BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSpecialSkillIdList[i]]));
                }
            }

            //总概率计算
            for (int i = 0; i < BattleAttackSkillList.Count; i++)
            {
                AllAttackSkillProp += BattleAttackSkillList[i].TriggerProb;
            }
            for (int i = 0; i < BattleDefendSkillList.Count; i++)
            {
                AllDefendSkillProp += BattleDefendSkillList[i].TriggerProb;
            }
        }
        public RoleBattleData(IRoleBattleData buffRoleBattleData,MachineData machineData)
        {
            attack = machineData.Atk;
            maxHealth = machineData.Hp;
            health = machineData.Hp;
            defence = machineData.Defense;
            maxEndurance = machineData.Mp;
            endurance = machineData.Mp;
            enduranceReply = machineData.MpReply;
            actionBar = machineData.Speed;
            critProp = machineData.Crt;
            dodgeProp = machineData.Eva;
            receiveDamage = 100 - machineData.ReduceAtk;
            pierce = machineData.ReduceDef;
            reboundDamage = machineData.Rebound;
            critDamage = machineData.CrtAtk;
            critResistance = machineData.CrtDef;
            this.buffRoleBattleData = buffRoleBattleData;

            Dictionary<int, BattleAttackSkillData> tempAttackSkillDict = GameManager.CustomeModule<BattleRoomManager>().battleAttackSkillDataDict;
            List<BattleAttackSkillData> tempAttackSkillList = tempAttackSkillDict.Values.ToList();

            BattleAttackSkillList = new List<BattleSkill>();
            BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[4000], 1));
            BattleDefendSkillList = new List<BattleSkill>();

            for (int i = 0; i < machineData.SkillPool.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(machineData.SkillPool[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[machineData.SkillPool[i]];
                    if (battleAttackSkillData.isAttackSkill)
                        BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                    else
                        BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                }
            }

            //总概率计算
            for (int i = 0; i < BattleAttackSkillList.Count; i++)
            {
                AllAttackSkillProp += BattleAttackSkillList[i].TriggerProb;
            }
            for (int i = 0; i < BattleDefendSkillList.Count; i++)
            {
                AllDefendSkillProp += BattleDefendSkillList[i].TriggerProb;
            }
        }
    }
}
