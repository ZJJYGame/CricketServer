using System;
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
        BattleCharacterEntity BattleCharacterEntity { get; set; }
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
        float critProp;
        public float CritProp
        {
            get
            {
                float num = critProp + buffRoleBattleData.CritProp;
                return num > 80 ? 80 : num;
            }
        }
        //闪避率
        float dodgeProp;
        public float DodgeProp { get { return dodgeProp + buffRoleBattleData.DodgeProp; } }
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

        public List<BattleSkill> BattlePassiveSkillList { get; private set; } 

        IRoleBattleData buffRoleBattleData;

        /// <summary>
        /// 受到的直接伤害
        /// </summary>
        //public void OnHurt(BattleDamageData battleDamageData)
        //{
        //    for (int i = 0; i < battleDamageData.damageNumList.Count; i++)
        //    {
        //        health -= battleDamageData.damageNumList[i];
        //    }
        //}
        public void OnHurt(int damage)
        {
            health -= damage;
            if (health <= 0) BattleCharacterEntity.IsWin = false;
        }
        /// <summary>
        /// 受到的反伤伤害
        /// </summary>
        //public void OnReboundHurt(BattleDamageData battleDamageData)
        //{
        //    for (int i = 0; i < battleDamageData.returnDamageNumList.Count; i++)
        //    {
        //        health -= battleDamageData.returnDamageNumList[i];
        //    }
        //}
        public void OnReboundHurt(int damage)
        {
            health -= damage;
            if (health <= 0) BattleCharacterEntity.IsWin = false;
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

        public RoleBattleData(IRoleBattleData buffRoleBattleData,RoleDTO roleDTO,CricketDTO cricketDTO,BattleCharacterEntity battleCharacterEntity)
        {
            BattleCharacterEntity = battleCharacterEntity;

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
            critProp = cricketstatus.Crt*100;
            dodgeProp = cricketstatus.Eva*100;
            receiveDamage = 100 - (int)cricketstatus.ReduceAtk;
            pierce = (int)cricketstatus.ReduceDef;
            reboundDamage = (int)cricketstatus.Rebound;
            critDamage = (int)cricketstatus.CrtAtk;
            critResistance = (int)cricketstatus.CrtDef;
            this.buffRoleBattleData = buffRoleBattleData;
            Dictionary<int, BattleAttackSkillData> tempAttackSkillDict = GameManager.CustomeModule<BattleRoomManager>().battleAttackSkillDataDict;
            List<BattleAttackSkillData> tempAttackSkillList = tempAttackSkillDict.Values.ToList();

            BattleAttackSkillList = new List<BattleSkill>();
            BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[4000],1));//添加普通攻击技能
            //BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[3001], 1));
            //BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[3003], 1));
            //BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[3005], 1));
            BattleDefendSkillList = new List<BattleSkill>();
            BattlePassiveSkillList = new List<BattleSkill>();
            //BattlePassiveSkillList.Add(new BattleSkill(tempAttackSkillDict[3332], 1));

            var tempSkillIdList = cricketDTO.SkillDict.Keys.ToList();
            var tempSpecialSkillIdList = cricketDTO.SpecialDict.Keys.ToList();
            for (int i = 0; i < tempSkillIdList.Count; i++)
            {
                Utility.Debug.LogError("蛐蛐技能id" + tempSkillIdList[i]);
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(tempSkillIdList[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[tempSkillIdList[i]];
                    switch (battleAttackSkillData.battleSkillType)
                    {
                        case BattleSkillType.AttackSkill:
                            Utility.Debug.LogError("攻击技能添加成功");
                            BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                        case BattleSkillType.BeAttackSkill:
                            BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                        case BattleSkillType.PassiveSkill:
                            BattlePassiveSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                    }
                }
            }
            for (int i = 0; i < tempSpecialSkillIdList.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(tempSpecialSkillIdList[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[tempSpecialSkillIdList[i]];
                    switch (battleAttackSkillData.battleSkillType)
                    {
                        case BattleSkillType.AttackSkill:
                            BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                        case BattleSkillType.BeAttackSkill:
                            BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                        case BattleSkillType.PassiveSkill:
                            BattlePassiveSkillList.Add(new BattleSkill(battleAttackSkillData, cricketDTO.SkillDict[tempSkillIdList[i]]));
                            break;
                    }
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
        public RoleBattleData(IRoleBattleData buffRoleBattleData,MachineData machineData,BattleCharacterEntity battleCharacterEntity)
        {
            BattleCharacterEntity = battleCharacterEntity;

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
            BattlePassiveSkillList = new List<BattleSkill>();

            for (int i = 0; i < machineData.SkillPool.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(machineData.SkillPool[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[machineData.SkillPool[i]];
                    switch (battleAttackSkillData.battleSkillType)
                    {
                        case BattleSkillType.AttackSkill:
                            BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                        case BattleSkillType.BeAttackSkill:
                            BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                        case BattleSkillType.PassiveSkill:
                            BattlePassiveSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                    }
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
        public RoleBattleData(IRoleBattleData buffRoleBattleData, TowerRobotData towerRobotData, BattleCharacterEntity battleCharacterEntity)
        {
            BattleCharacterEntity = battleCharacterEntity;

            attack = towerRobotData.Atk;
            maxHealth = towerRobotData.Hp;
            health = towerRobotData.Hp;
            defence = towerRobotData.Defense;
            maxEndurance = towerRobotData.Mp;
            endurance = towerRobotData.Mp;
            enduranceReply = towerRobotData.MpReply;
            actionBar = towerRobotData.Speed;
            critProp = towerRobotData.Crt;
            dodgeProp = towerRobotData.Eva;
            receiveDamage = 100 - towerRobotData.ReduceAtk;
            pierce = towerRobotData.ReduceDef;
            reboundDamage = towerRobotData.Rebound;
            critDamage = towerRobotData.CrtAtk;
            critResistance = towerRobotData.CrtDef;
            this.buffRoleBattleData = buffRoleBattleData;

            Dictionary<int, BattleAttackSkillData> tempAttackSkillDict = GameManager.CustomeModule<BattleRoomManager>().battleAttackSkillDataDict;
            List<BattleAttackSkillData> tempAttackSkillList = tempAttackSkillDict.Values.ToList();

            BattleAttackSkillList = new List<BattleSkill>();
            BattleAttackSkillList.Add(new BattleSkill(tempAttackSkillDict[4000], 1));
            BattleDefendSkillList = new List<BattleSkill>();
            BattlePassiveSkillList = new List<BattleSkill>();

            for (int i = 0; i < towerRobotData.SkillPool.Count; i++)
            {
                //攻击受击技能添加
                if (tempAttackSkillDict.ContainsKey(towerRobotData.SkillPool[i]))
                {
                    BattleAttackSkillData battleAttackSkillData = tempAttackSkillDict[towerRobotData.SkillPool[i]];
                    switch (battleAttackSkillData.battleSkillType)
                    {
                        case BattleSkillType.AttackSkill:
                            BattleAttackSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                        case BattleSkillType.BeAttackSkill:
                            BattleDefendSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                        case BattleSkillType.PassiveSkill:
                            BattlePassiveSkillList.Add(new BattleSkill(battleAttackSkillData, 1));
                            break;
                    }
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
