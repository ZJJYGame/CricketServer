using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;

namespace AscensionServer
{
    public class BattleController
    {
        //当前战斗时间
        int nowTime;

        BattleCharacterEntity playerOne;
        BattleCharacterEntity playerTwo;

        //碰撞相关数据
        int crashNum = 0;//碰撞次数
        int crashColdTime=5000;//碰撞冷却时间

        bool CanCrash { get { return crashNum < 3 || crashColdTime <= 0; } }

        BattleTransferDTO battleTransferDTO = new BattleTransferDTO();
        List<BattleRoleActionData> battleRoleActionDataList = new List<BattleRoleActionData>();

        public void StartBattle()
        {
            SetTransferRoleData();

            BattleCharacterEntity attackPlayer;
            BattleCharacterEntity defendPlayer;
            BattleCharacterEntity nextAttackPlayer=null;
            BattleCharacterEntity nextDefendPlayer=null;
            //双方血量大于0一直执行
            Utility.Debug.LogInfo("开始战斗流程");
            //双方被动技能生效
            playerOne.UsePassiveSkill();
            playerTwo.UsePassiveSkill();

            while (playerOne.roleBattleData.Health > 0 && playerTwo.roleBattleData.Health > 0)
            {
                bool isCrash = false;
               
                //决定攻击方
                if (playerOne.RemainActionBar < playerTwo.RemainActionBar)
                {
                    attackPlayer = playerOne;
                    defendPlayer = playerTwo;
                    if (CanCrash&& Math.Abs(playerOne.RemainActionBar - playerTwo.RemainActionBar) <= 20 && playerOne.roleBattleData.Endurance > 0 && playerTwo.roleBattleData.Endurance > 0)
                        isCrash = true;
                }
                else if(playerOne.RemainActionBar > playerTwo.RemainActionBar)
                {
                    attackPlayer = playerTwo;
                    defendPlayer = playerOne;
                    if (CanCrash&& Math.Abs(playerOne.RemainActionBar - playerTwo.RemainActionBar) <= 20 && playerOne.roleBattleData.Endurance > 0 && playerTwo.roleBattleData.Endurance > 0)
                        isCrash = true;
                }
                else
                {
                    if (CanCrash&& Math.Abs(playerOne.RemainActionBar - playerTwo.RemainActionBar) <= 20 && playerOne.roleBattleData.Endurance > 0 && playerTwo.roleBattleData.Endurance > 0)
                        isCrash = true;
                    if (!isCrash)
                    {
                        if (nextAttackPlayer != null)
                        {
                            attackPlayer = nextAttackPlayer;
                            defendPlayer = nextDefendPlayer;
                            nextAttackPlayer = null;
                            nextDefendPlayer = null;
                        }
                        else
                        {
                            int index = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, 2);
                            if (index == 0)
                            {
                                attackPlayer = playerOne;
                                defendPlayer = playerTwo;
                                nextAttackPlayer = playerTwo;
                                nextDefendPlayer = playerOne;
                            }
                            else
                            {
                                attackPlayer = playerTwo;
                                defendPlayer = playerOne;
                                nextAttackPlayer = playerOne;
                                nextDefendPlayer = playerTwo;
                            }
                        }
                    }
                    else
                    {
                        attackPlayer = playerOne;
                        defendPlayer = playerTwo;
                    }
                }
                //总时间增加 
                int oldTime = nowTime;
                nowTime += attackPlayer.RemainActionBar;
                int offestTime = nowTime - oldTime;
                //行动条结算
                attackPlayer.ChangeActionBar(offestTime);
                defendPlayer.ChangeActionBar(offestTime);
                if (isCrash)
                    defendPlayer.ChangeActionBar(-defendPlayer.roleBattleData.ActionBar);
 
                //todo所有buff实体持续时间减少
                attackPlayer.battleBuffController.UpdateBuffTime(offestTime);
                defendPlayer.battleBuffController.UpdateBuffTime(offestTime);

                if (!isCrash)
                {
                    List<BattleSkill> defendBattleSkillList = new List<BattleSkill>();
                    PlayerAction(attackPlayer, defendPlayer, defendBattleSkillList,out var attackBattleDamageData,out var defendBattleDamageDataList, isCrash);

                    
                    DamageTakeEffect(attackPlayer, defendPlayer, attackBattleDamageData, null);

                    List<int> defendTriggerSkillList = new List<int>();
                    for (int i = 0; i < defendBattleSkillList.Count; i++)
                    {
                        if (defendBattleSkillList[i] != null)
                            defendTriggerSkillList.Add(defendBattleSkillList[i].SkillId);
                    }

                    battleRoleActionDataList.Add(GetTransferData(new List<BattleDamageData>() { attackBattleDamageData }, null, defendBattleSkillList, new List<int>() { attackPlayer.RoleID }, false));

                    crashColdTime -= offestTime;

                    AddAllBuff(attackBattleDamageData, attackPlayer, defendPlayer, defendBattleDamageDataList);
                }
                else//碰撞
                {
                    List<BattleSkill> defendBattleSkillList = new List<BattleSkill>();
                    PlayerAction(attackPlayer, defendPlayer, defendBattleSkillList, out var attackBattleDamageData,out  var attackDefendBattleDamageDataList, isCrash);

                    List<int> defendTriggerSkillList = new List<int>();
                    for (int i = 0; i < defendBattleSkillList.Count; i++)
                    {
                        if (defendBattleSkillList[i] != null)
                            defendTriggerSkillList.Add(defendBattleSkillList[i].SkillId);
                    }

                    List<BattleSkill> attackBattleSkillList = new List<BattleSkill>();
                    PlayerAction(defendPlayer, attackPlayer, attackBattleSkillList, out var defendBattleDamageData,out var defendAttackBattleDamageDataList, isCrash);

                    DamageTakeEffect(attackPlayer, defendPlayer, attackBattleDamageData, defendBattleDamageData);

                    List<int> attackTriggerSkillList = new List<int>();
                    for (int i = 0; i < attackBattleSkillList.Count; i++)
                    {
                        if (attackBattleSkillList[i] != null)
                            attackTriggerSkillList.Add(attackBattleSkillList[i].SkillId);
                    }

                    battleRoleActionDataList.Add(GetTransferData(new List<BattleDamageData>() { attackBattleDamageData, defendBattleDamageData }, attackBattleSkillList, defendBattleSkillList, new List<int>() { attackPlayer.RoleID,defendPlayer.RoleID }, true));

                    crashNum += 1;
                    crashColdTime = 5000;

                    AddAllBuff(attackBattleDamageData, attackPlayer, defendPlayer, attackDefendBattleDamageDataList);
                    AddAllBuff(defendBattleDamageData, defendPlayer, attackPlayer, defendAttackBattleDamageDataList);
                }
                //触发buff
                attackPlayer.battleBuffController.TriggerBuff();
                defendPlayer.battleBuffController.TriggerBuff();

                Utility.Debug.LogError("当前时间=>" + nowTime + (isCrash ? "，发生碰撞" : "，没有碰撞") + ",碰撞冷却=>" + crashColdTime);
            }

            Utility.Debug.LogInfo("战斗结果：蛐蛐" + playerOne.CricketID + (playerOne.IsWin ? "胜利" : "失败") + "，蛐蛐" + playerTwo.CricketID + (playerTwo.IsWin ? "胜利" : "失败"));
            battleTransferDTO.BattleRoleActionDataList = battleRoleActionDataList;
            Utility.Debug.LogInfo("战斗传输数据=>" + Utility.Json.ToJson(battleTransferDTO));
            Dictionary<int, BattleResult> battleResultDict = GameManager.CustomeModule<MatchManager>().BattleCombat(playerOne, playerTwo);
            if (battleResultDict.ContainsKey(battleTransferDTO.RoleOneData.RoleID))
                battleTransferDTO.RoleOneData.BattleResult = battleResultDict[battleTransferDTO.RoleOneData.RoleID];
            if (battleResultDict.ContainsKey(battleTransferDTO.RoleTwoData.RoleID))
                battleTransferDTO.RoleTwoData.BattleResult = battleResultDict[battleTransferDTO.RoleTwoData.RoleID];
            playerOne.S2CSendBattleData(battleTransferDTO);
            playerTwo.S2CSendBattleData(battleTransferDTO);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackPlayer"></param>
        /// <param name="defendPlayer"></param>
        /// <param name="defendBattleSkillList">受击技能列表</param>
        /// <param name="attackBattleDamageData">攻击者技能伤害信息</param>
        void PlayerAction(BattleCharacterEntity attackPlayer,BattleCharacterEntity defendPlayer, List<BattleSkill> defendBattleSkillList,out BattleDamageData attackBattleDamageData,out List<BattleDamageData> defendBattleDamageDataList, bool isCrash)
        {
            BattleSkill attackBattleSkill = attackPlayer.RandomSkill(true);
            if (attackBattleSkill == null)
                attackPlayer.roleBattleData.OnEnduranceReply();
            attackBattleDamageData = GetDamageData(attackBattleSkill, attackPlayer, defendPlayer, !isCrash,true);
             defendBattleDamageDataList = new List<BattleDamageData>();
            for (int i = 0; i < attackBattleDamageData.isDodgeList.Count; i++)
            {
                if (!attackBattleDamageData.isDodgeList[i])
                {
                    BattleSkill defendBattleSkill = defendPlayer.RandomSkill(false);
                    BattleDamageData defendBattleDamageData = GetDamageData(defendBattleSkill, defendPlayer, attackPlayer, true,false);
                    defendBattleSkillList.Add(defendBattleSkill);
                    defendBattleDamageDataList.Add(defendBattleDamageData);
                }
            }
            //耐力消耗
            attackPlayer.roleBattleData.OnEnduranceCost(attackBattleSkill);
            for (int i = 0; i < defendBattleSkillList.Count; i++)
            {
                defendPlayer.roleBattleData.OnEnduranceCost(defendBattleSkillList[i]);
            }
            //显示
            if(attackBattleSkill!=null)
            Utility.Debug.LogInfo(attackPlayer.CricketID + "使用技能" + attackBattleSkill.SkillId + "攻击了" + defendPlayer.CricketID);
            for (int i = 0; i < attackBattleDamageData.damageNumList.Count; i++)
            {
                if (attackBattleDamageData.isDodgeList[i])
                    Utility.Debug.LogInfo("闪避了");
                else
                    Utility.Debug.LogInfo("造成了" + attackBattleDamageData.damageNumList[i] + "点伤害" + (attackBattleDamageData.isCritList[i] ? "暴击" : "没有暴击"));
            }


            //恢复进度条
            attackPlayer.TryRestartActionBar();
        }

        void AddAllBuff(BattleDamageData attackBattleDamageData, BattleCharacterEntity attackPlayer, BattleCharacterEntity defendPlayer,List<BattleDamageData> defendBattleDamageDataList)
        {
            for (int i = 0; i < attackBattleDamageData.battleSkillAddBuffList.Count; i++)
            {
                if (attackBattleDamageData.battleSkillAddBuffList[i].TargetSelf)//对自己
                {
                    attackPlayer.battleBuffController.AddBuff(attackBattleDamageData.battleSkillAddBuffList[i], attackBattleDamageData.skillId);
                }
                else
                {
                    defendPlayer.battleBuffController.AddBuff(attackBattleDamageData.battleSkillAddBuffList[i], attackBattleDamageData.skillId);
                }
            }
            for (int i = 0; i < defendBattleDamageDataList.Count; i++)
            {
                for (int j = 0; j < defendBattleDamageDataList[i].battleSkillAddBuffList.Count; j++)
                {
                    if (defendBattleDamageDataList[i].battleSkillAddBuffList[j].TargetSelf)//对自己
                    {
                        defendPlayer.battleBuffController.AddBuff(defendBattleDamageDataList[i].battleSkillAddBuffList[j], defendBattleDamageDataList[i].skillId);
                    }
                    else
                    {
                        attackPlayer.battleBuffController.AddBuff(defendBattleDamageDataList[i].battleSkillAddBuffList[j], defendBattleDamageDataList[i].skillId);
                    }
                }
            }
        }

        //获取伤害数据
        BattleDamageData GetDamageData(BattleSkill battleSkill, BattleCharacterEntity attackPlayer, BattleCharacterEntity defendPlayer,bool canDodge,bool isInitiative)
        {
            RoleBattleData attackPlayerData = attackPlayer.roleBattleData;
            RoleBattleData defendPlayerData = defendPlayer.roleBattleData;
            BattleDamageData battleDamageData = new BattleDamageData();
            if (battleSkill != null)
            {
                battleDamageData.skillId = battleSkill.SkillId;
                battleDamageData.endurenceReply = -battleSkill.EnduranceCost;
                int atk = (int)(attackPlayerData.Attack * battleSkill.DamagePercentValue / 100f);
                int normalDamage = (int)((defendPlayerData.ReceiveDamage / 100f) * (atk - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
                int critDamage = (int)((defendPlayerData.ReceiveDamage / 100f) * (atk+(atk * attackPlayerData.CritDamage * (100 - defendPlayerData.CritResistance) / 10000f) - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
                normalDamage = normalDamage <= 0 ? 1 : normalDamage;
                critDamage = critDamage <= 0 ? 1 : critDamage;


                for (int i = 0; i < battleSkill.AttackNumber; i++)
                {
                    int dodgeRandomIndex = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, 101);
                    if (canDodge && dodgeRandomIndex <= defendPlayerData.DodgeProp)//闪避成功
                    {
                        battleDamageData.damageNumList.Add(0);
                        battleDamageData.isCritList.Add(false);
                        battleDamageData.isDodgeList.Add(true);
                        battleDamageData.returnDamageNumList.Add(0);
                    }
                    else//闪避失败
                    {
                        battleDamageData.isDodgeList.Add(false);
                        int critRandomIndex = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, 101);
                        if (critRandomIndex <= attackPlayerData.CritProp)//暴击了
                        {
                            battleDamageData.damageNumList.Add(critDamage);
                            battleDamageData.returnDamageNumList.Add((int)(critDamage * defendPlayerData.ReboundDamage / 100f));
                            battleDamageData.isCritList.Add(true);
                        }
                        else//没暴击
                        {
                            battleDamageData.damageNumList.Add(normalDamage);
                            battleDamageData.returnDamageNumList.Add((int)(normalDamage * defendPlayerData.ReboundDamage / 100f));
                            battleDamageData.isCritList.Add(false);
                        }
                    }
                }
                for (int i = 0; i < battleSkill.BattleSkillAddBuffList.Count; i++)
                {
                    battleDamageData.battleSkillAddBuffList.Add(battleSkill.BattleSkillAddBuffList[i]);
                }
            }
            else
            {
                if (isInitiative)
                    battleDamageData.endurenceReply = attackPlayer.roleBattleData.EnduranceReply;
            }
            return battleDamageData;
        }
        //设置一次行为的传输数据
        BattleRoleActionData GetTransferData(List<BattleDamageData> battleDamageDataList,List<BattleSkill> attackTriggerSkillList,List<BattleSkill> defendTriggerList,List<int> roleIdList,bool isCrash)
        {
            BattleRoleActionData battleRoleActionData = new BattleRoleActionData();
            battleRoleActionData.Time = nowTime;
            battleRoleActionData.IsCrash = isCrash;
            BattleDamageData attackDamageData = battleDamageDataList[0];
            BattleActionData attackBattleActionData = battleRoleActionData.AttackBattleActionData;
            attackBattleActionData.EndurenceReply = attackDamageData.endurenceReply;
            attackBattleActionData.RoleId = roleIdList[0];
            attackBattleActionData.SkillId = attackDamageData.skillId;
            for (int i = 0; i < attackDamageData.damageNumList.Count; i++)
                attackBattleActionData.DamageList.Add(attackDamageData.damageNumList[i]);
            for (int i = 0; i < attackDamageData.isCritList.Count; i++)
                attackBattleActionData.IsCritList.Add(attackDamageData.isCritList[i]);
            for (int i = 0; i < attackDamageData.isDodgeList.Count; i++)
                attackBattleActionData.IsDodgeList.Add(attackDamageData.isDodgeList[i]);
            for (int i = 0; i < attackDamageData.returnDamageNumList.Count; i++)
                attackBattleActionData.ReturnDamageList.Add(attackDamageData.returnDamageNumList[i]);
            for (int i = 0; i < attackDamageData.battleSkillAddBuffList.Count; i++)
            {
                attackBattleActionData.AddBuffList.Add(attackDamageData.battleSkillAddBuffList[i].BuffId);
                attackBattleActionData.AddBuffDurationTime.Add(attackDamageData.battleSkillAddBuffList[i].DurationTime);
            }
            if (defendTriggerList != null)
            {
                for (int i = 0; i < defendTriggerList.Count; i++)
                {
                    if (defendTriggerList[i] != null)
                    {
                        TriggerSkillData triggerSkillData = new TriggerSkillData();
                        triggerSkillData.SkillId = defendTriggerList[i].SkillId;
                        triggerSkillData.EnduranceCost = defendTriggerList[i].EnduranceCost;
                        for (int j = 0; j < defendTriggerList[i].BattleSkillAddBuffList.Count; j++)
                        {
                            triggerSkillData.AddBuffId.Add(defendTriggerList[i].BattleSkillAddBuffList[j].BuffId);
                            triggerSkillData.DurationTime.Add(defendTriggerList[i].BattleSkillAddBuffList[j].DurationTime);
                        }
                        attackBattleActionData.TriggerSkillList.Add(triggerSkillData);
                    }
                }
            }
            if (isCrash)
            {
                BattleDamageData defendDamageData = battleDamageDataList[1];
                Utility.Debug.LogError("耐力消耗=>" + defendDamageData.endurenceReply);
                BattleActionData defendBattleActionData = battleRoleActionData.DefendBattleActionData;
                defendBattleActionData.EndurenceReply = defendDamageData.endurenceReply;
                defendBattleActionData.RoleId = roleIdList[1];
                defendBattleActionData.SkillId = defendDamageData.skillId;
                for (int i = 0; i < defendDamageData.damageNumList.Count; i++)
                    defendBattleActionData.DamageList.Add(defendDamageData.damageNumList[i]);
                for (int i = 0; i < defendDamageData.isCritList.Count; i++)
                    defendBattleActionData.IsCritList.Add(defendDamageData.isCritList[i]);
                for (int i = 0; i < defendDamageData.isDodgeList.Count; i++)
                    defendBattleActionData.IsDodgeList.Add(defendDamageData.isDodgeList[i]);
                for (int i = 0; i < defendDamageData.returnDamageNumList.Count; i++)
                    defendBattleActionData.ReturnDamageList.Add(defendDamageData.returnDamageNumList[i]);
                for (int i = 0; i < defendDamageData.battleSkillAddBuffList.Count; i++)
                {
                    defendBattleActionData.AddBuffList.Add(defendDamageData.battleSkillAddBuffList[i].BuffId);
                    defendBattleActionData.AddBuffDurationTime.Add(defendDamageData.battleSkillAddBuffList[i].DurationTime);
                }
                if (attackTriggerSkillList != null)
                {
                    for (int i = 0; i < attackTriggerSkillList.Count; i++)
                    {
                        if (attackTriggerSkillList[i] != null)
                        {
                            TriggerSkillData triggerSkillData = new TriggerSkillData();
                            triggerSkillData.SkillId = attackTriggerSkillList[i].SkillId;
                            triggerSkillData.EnduranceCost = attackTriggerSkillList[i].EnduranceCost;
                            for (int j = 0; j < attackTriggerSkillList[i].BattleSkillAddBuffList.Count; j++)
                            {
                                triggerSkillData.AddBuffId.Add(attackTriggerSkillList[i].BattleSkillAddBuffList[j].BuffId);
                                triggerSkillData.DurationTime.Add(attackTriggerSkillList[i].BattleSkillAddBuffList[j].DurationTime);
                            }
                            defendBattleActionData.TriggerSkillList.Add(triggerSkillData); 
                        }
                    }
                }
            }
            else
            {
                battleRoleActionData.DefendBattleActionData = null;
            }
            return battleRoleActionData;
        }
        //每轮伤害结算
        void DamageTakeEffect(BattleCharacterEntity attacker,BattleCharacterEntity defender,BattleDamageData attackerDamage,BattleDamageData defenderDamage)
        {
            if(defenderDamage==null)//非碰撞的情况
            {
                for (int i = 0; i < attackerDamage.damageNumList.Count; i++)
                {
                    defender.roleBattleData.OnHurt(attackerDamage.damageNumList[i]);
                    attacker.roleBattleData.OnReboundHurt(attackerDamage.returnDamageNumList[i]);
                }
            }
            else//碰撞的情况
            {
                int count = attackerDamage.damageNumList.Count > defenderDamage.damageNumList.Count ? attackerDamage.damageNumList.Count : defenderDamage.damageNumList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (attackerDamage.damageNumList.Count > i )
                    {
                        defender.roleBattleData.OnHurt(attackerDamage.damageNumList[i]);
                        attacker.roleBattleData.OnReboundHurt(attackerDamage.returnDamageNumList[i]);
                    }
                    if (defenderDamage.damageNumList.Count > i )
                    {
                        attacker.roleBattleData.OnHurt(defenderDamage.damageNumList[i]);
                        defender.roleBattleData.OnReboundHurt(defenderDamage.returnDamageNumList[i]);
                    }
                    if (attacker.roleBattleData.Health<=0 || defender.roleBattleData.Health<=0)//有一方死亡
                    {
                        Utility.Debug.LogError("碰撞中有一方死亡");
                        if (attackerDamage.damageNumList.Count > i + 1)
                            attackerDamage.damageNumList.RemoveRange(i + 1, attackerDamage.damageNumList.Count - (i + 1));
                        if (attackerDamage.returnDamageNumList.Count > i + 1)
                            attackerDamage.returnDamageNumList.RemoveRange(i + 1, attackerDamage.returnDamageNumList.Count - (i + 1));
                        if (defenderDamage.damageNumList.Count > i + 1)
                            defenderDamage.damageNumList.RemoveRange(i + 1, defenderDamage.damageNumList.Count - (i + 1));
                        if (defenderDamage.returnDamageNumList.Count > i + 1)
                            defenderDamage.returnDamageNumList.RemoveRange(i + 1, defenderDamage.returnDamageNumList.Count - (i + 1));
                        break;
                    }
                }
            }
        }

        public void SetTransferRoleData()
        {
            battleTransferDTO.RoleOneData = new BattleRoleData()
            {
                RoleID = playerOne.RoleID,
                RoleName = playerOne.RoleName,
                CricketId = playerOne.CricketID,
                MaxHealth = playerOne.roleBattleData.MaxHealth,
                Health = playerOne.roleBattleData.Health,
                MaxEndurance = playerOne.roleBattleData.MaxEndurance,
                Endurance = playerOne.roleBattleData.Endurance,
                ActionBar = playerOne.roleBattleData.ActionBar,
                PassiveSkill = new List<TriggerSkillData>(),
            };
            for (int i = 0; i < playerOne.roleBattleData.BattlePassiveSkillList.Count; i++)
            {

            }
            battleTransferDTO.RoleTwoData = new BattleRoleData()
            {
                RoleID = playerTwo.RoleID,
                RoleName = playerTwo.RoleName,
                CricketId = playerTwo.CricketID,
                MaxHealth = playerTwo.roleBattleData.MaxHealth,
                Health = playerTwo.roleBattleData.Health,
                MaxEndurance = playerTwo.roleBattleData.MaxEndurance,
                Endurance = playerTwo.roleBattleData.Endurance,
                ActionBar = playerTwo.roleBattleData.ActionBar,
                PassiveSkill = new List<TriggerSkillData>(),
            };
        }

        public void InitController(BattleCharacterEntity battleCharacterEntityOne,BattleCharacterEntity battleCharacterEntityTwo)
        {
            playerOne = battleCharacterEntityOne;
            playerTwo = battleCharacterEntityTwo;
        }
    }
}
