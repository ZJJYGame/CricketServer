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
        //当前总攻击次数
        int nowAttackNum;
        BattleCharacterEntity playerOne;
        BattleCharacterEntity playerTwo;

        //碰撞相关数据
        int crashNum = 0;//碰撞次数
        int crashColdTime=5000;//碰撞冷却时间

        bool CanCrash { get { return crashNum < 3 || crashColdTime <= 0; } }

        List<BattleRoleActionData> battleRoleActionDataList = new List<BattleRoleActionData>();

        public void StartBattle()
        {
            BattleCharacterEntity attackPlayer;
            BattleCharacterEntity defendPlayer;
            BattleCharacterEntity nextAttackPlayer=null;
            BattleCharacterEntity nextDefendPlayer=null;
            //双方血量大于0一直执行
            Utility.Debug.LogInfo("开始战斗流程");
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
                    PlayerAction(attackPlayer, defendPlayer, defendBattleSkillList,out var attackBattleDamageData,isCrash);

                    List<int> defendTriggerSkillList = new List<int>();
                    for (int i = 0; i < defendBattleSkillList.Count; i++)
                    {
                        if (defendBattleSkillList[i] != null)
                            defendTriggerSkillList.Add(defendBattleSkillList[i].SkillId);
                    }

                    battleRoleActionDataList.Add(GetTransferData(new List<BattleDamageData>() { attackBattleDamageData }, null, defendTriggerSkillList, new List<int>() { attackPlayer.RoleId }, false));

                    crashColdTime -= offestTime;
                }
                else//碰撞
                {
                    List<BattleSkill> defendBattleSkillList = new List<BattleSkill>();
                    PlayerAction(attackPlayer, defendPlayer, defendBattleSkillList, out var attackBattleDamageData,isCrash);

                    List<int> defendTriggerSkillList = new List<int>();
                    for (int i = 0; i < defendBattleSkillList.Count; i++)
                    {
                        if (defendBattleSkillList[i] != null)
                            defendTriggerSkillList.Add(defendBattleSkillList[i].SkillId);
                    }

                    List<BattleSkill> attackBattleSkillList = new List<BattleSkill>();
                    PlayerAction(defendPlayer, attackPlayer, attackBattleSkillList, out var defendBattleDamageData,isCrash);

                    List<int> attackTriggerSkillList = new List<int>();
                    for (int i = 0; i < attackBattleSkillList.Count; i++)
                    {
                        if (attackBattleSkillList[i] != null)
                            attackTriggerSkillList.Add(attackBattleSkillList[i].SkillId);
                    }

                    battleRoleActionDataList.Add(GetTransferData(new List<BattleDamageData>() { attackBattleDamageData, defendBattleDamageData }, attackTriggerSkillList, defendTriggerSkillList, new List<int>() { attackPlayer.RoleId,defendPlayer.RoleId }, true));

                    crashNum += 1;
                    crashColdTime = 5000;
                }
                Utility.Debug.LogError("当前时间=>" + nowTime + (isCrash ? "，发生碰撞" : "，没有碰撞") + ",碰撞冷却=>" + crashColdTime);
            }
            Utility.Debug.LogInfo("战斗计算流程结束");
            Utility.Debug.LogInfo("战斗传输数据=>" + Utility.Json.ToJson(battleRoleActionDataList));
            GameManager.CustomeModule<BattleRoomManager>().S2CEnterBattle(playerOne.RoleId, battleRoleActionDataList);
            GameManager.CustomeModule<BattleRoomManager>().S2CEnterBattle(playerTwo.RoleId, battleRoleActionDataList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackPlayer"></param>
        /// <param name="defendPlayer"></param>
        /// <param name="defendBattleSkillList">受击技能列表</param>
        /// <param name="attackBattleDamageData">攻击者技能伤害信息</param>
        void PlayerAction(BattleCharacterEntity attackPlayer,BattleCharacterEntity defendPlayer, List<BattleSkill> defendBattleSkillList,out BattleDamageData attackBattleDamageData,bool isCrash)
        {
            BattleSkill attackBattleSkill = attackPlayer.RandomSkill(true);
            if (attackBattleSkill == null)
                attackPlayer.roleBattleData.OnEnduranceReply();
            attackBattleDamageData = GetDamageData(attackBattleSkill, attackPlayer, defendPlayer, !isCrash,true);
            List<BattleDamageData> defendBattleDamageDataList = new List<BattleDamageData>();
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
            //伤害结算
            defendPlayer.roleBattleData.OnHurt(attackBattleDamageData);
            attackPlayer.roleBattleData.OnReboundHurt(attackBattleDamageData);
            //耐力消耗
            attackPlayer.roleBattleData.OnEnduranceCost(attackBattleSkill);
            for (int i = 0; i < defendBattleSkillList.Count; i++)
            {
                defendPlayer.roleBattleData.OnEnduranceCost(defendBattleSkillList[i]);
            }
            //显示
            if(attackBattleSkill!=null)
            Utility.Debug.LogInfo(attackPlayer.RoleId + "使用技能" + attackBattleSkill.SkillId + "攻击了" + defendPlayer.RoleId);
            for (int i = 0; i < attackBattleDamageData.damageNumList.Count; i++)
            {
                if (attackBattleDamageData.isDodgeList[i])
                    Utility.Debug.LogInfo("闪避了");
                else
                    Utility.Debug.LogInfo("造成了" + attackBattleDamageData.damageNumList[i] + "点伤害" + (attackBattleDamageData.isCritList[i] ? "暴击" : "没有暴击"));
            }
            //添加buff
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
            //触发buff
            attackPlayer.battleBuffController.TriggerBuff();
            defendPlayer.battleBuffController.TriggerBuff();

            //恢复进度条
            attackPlayer.TryRestartActionBar();
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
                int atk = (int)(attackPlayerData.Attack * battleSkill.DamagePercentValue / 100f);
                int normalDamage = (int)((defendPlayerData.ReceiveDamage / 100f) * (atk - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
                int critDamage = (int)((defendPlayerData.ReceiveDamage / 100f) * ((atk + atk * attackPlayerData.CritDamage * (100 - defendPlayerData.CritResistance) / 10000f) - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
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
        BattleRoleActionData GetTransferData(List<BattleDamageData> battleDamageDataList,List<int> attackTriggerSkillList,List<int> defendTriggerList,List<int> roleIdList,bool isCrash)
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
                attackBattleActionData.AddBuffList.Add(attackDamageData.battleSkillAddBuffList[i].BuffId);
            attackBattleActionData.TriggerSkillList = defendTriggerList;
            if (isCrash)
            {
                BattleDamageData defendDamageData = battleDamageDataList[1];
                BattleActionData defendBattleActionData = battleRoleActionData.DefendBattleActionData;
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
                    defendBattleActionData.AddBuffList.Add(defendDamageData.battleSkillAddBuffList[i].BuffId);
                defendBattleActionData.TriggerSkillList = attackTriggerSkillList;
            }
            else
            {
                battleRoleActionData.DefendBattleActionData = null;
            }
            return battleRoleActionData;
        }

        public void InitController(BattleCharacterEntity battleCharacterEntityOne,BattleCharacterEntity battleCharacterEntityTwo)
        {
            playerOne = battleCharacterEntityOne;
            playerTwo = battleCharacterEntityTwo;
        }
    }
}
