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
                //决定攻击方
                if (playerOne.RemainActionBar < playerTwo.RemainActionBar)
                {
                    attackPlayer = playerOne;
                    defendPlayer = playerTwo;
                }
                else if(playerOne.RemainActionBar > playerTwo.RemainActionBar)
                {
                    attackPlayer = playerTwo;
                    defendPlayer = playerOne;
                }
                else
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
                //总时间增加 
                int oldTime = nowTime;
                nowTime += attackPlayer.RemainActionBar;
                int offestTime = nowTime - oldTime;
                //行动条结算
                attackPlayer.ChangeActionBar(offestTime);
                defendPlayer.ChangeActionBar(offestTime);
                //todo所有buff实体持续时间减少
                attackPlayer.battleBuffController.UpdateBuffTime(offestTime);
                defendPlayer.battleBuffController.UpdateBuffTime(offestTime);

                BattleSkill battleSkill = attackPlayer.RandomSkill();
                BattleDamageData battleDamageData = GetDamageData(battleSkill, attackPlayer, defendPlayer);
                //伤害结算
                defendPlayer.roleBattleData.OnHurt(battleDamageData);
                attackPlayer.roleBattleData.OnReboundHurt(battleDamageData);
                //耐力消耗
                attackPlayer.roleBattleData.OnEnduranceCost(battleSkill);
                //显示
                Utility.Debug.LogInfo(attackPlayer.RoleId + "使用技能" + battleSkill.SkillId + "攻击了" + defendPlayer.RoleId);
                for (int i = 0; i < battleDamageData.damageNumList.Count; i++)
                {
                    if (battleDamageData.isDodgeList[i])
                        Utility.Debug.LogInfo("闪避了");
                    else
                        Utility.Debug.LogInfo("造成了" + battleDamageData.damageNumList[i] + "点伤害" + (battleDamageData.isCritList[i] ? "暴击" : "没有暴击"));
                }
                //添加buff
                for (int i = 0; i < battleDamageData.battleSkillAddBuffList.Count; i++)
                {
                    if (battleDamageData.battleSkillAddBuffList[i].TargetSelf)//对自己
                    {
                        Utility.Debug.LogInfo(attackPlayer.RoleId + "添加buff" + battleDamageData.battleSkillAddBuffList[i].BuffId);
                        attackPlayer.battleBuffController.AddBuff(battleDamageData.battleSkillAddBuffList[i], battleDamageData.skillId);
                    }
                    else
                    {
                        Utility.Debug.LogInfo(defendPlayer.RoleId + "添加buff" + battleDamageData.battleSkillAddBuffList[i].BuffId);
                        defendPlayer.battleBuffController.AddBuff(battleDamageData.battleSkillAddBuffList[i], battleDamageData.skillId);
                    }
                }
                //触发buff
                attackPlayer.battleBuffController.TriggerBuff();
                defendPlayer.battleBuffController.TriggerBuff();

                //恢复进度条
                attackPlayer.TryRestartActionBar();

                battleRoleActionDataList.Add(GetTransferData(new List<BattleDamageData>() { battleDamageData }, new List<int>() { attackPlayer.RoleId }, false));
                //Utility.Debug.LogInfo(attackPlayer.RoleId + "的行动条为" + attackPlayer.RemainActionBar + "," + defendPlayer.RoleId+"的行动条为" + defendPlayer.RemainActionBar);
            }
            Utility.Debug.LogInfo("战斗计算流程结束");
            Utility.Debug.LogInfo("战斗传输数据=>" + Utility.Json.ToJson(battleRoleActionDataList));
        }

        BattleDamageData GetDamageData(BattleSkill battleSkill, BattleCharacterEntity attackPlayer, BattleCharacterEntity defendPlayer)
        {
            RoleBattleData attackPlayerData = attackPlayer.roleBattleData;
            RoleBattleData defendPlayerData = defendPlayer.roleBattleData;
            BattleDamageData battleDamageData = new BattleDamageData() { skillId=battleSkill.SkillId};
            int atk = (int)(attackPlayerData.Attack * battleSkill.DamagePercentValue / 100f);
            int normalDamage = (int)((defendPlayerData.ReceiveDamage / 100f) * (atk - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f))+battleSkill.DamageFixedValue;
            int critDamage= (int)((defendPlayerData.ReceiveDamage / 100f) * ((atk+atk*attackPlayerData.CritDamage*(100-defendPlayerData.CritResistance)/10000f) - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
            for (int i = 0; i < battleSkill.AttackNumber; i++)
            {
                int dodgeRandomIndex = GameManager.CustomeModule<BattleRoomManager>().random.Next(0, 101);
                if (dodgeRandomIndex <= defendPlayerData.DodgeProp)//闪避成功
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
            return battleDamageData;
        }
        //设置一次行为的传输数据
        BattleRoleActionData GetTransferData(List<BattleDamageData> battleDamageDataList,List<int> roleIdList,bool isCrash)
        {
            BattleRoleActionData battleRoleActionData = new BattleRoleActionData();
            battleRoleActionData.Time = nowTime;
            battleRoleActionData.IsCrash = isCrash;
            BattleDamageData attackDamageData = battleDamageDataList[0];
            BattleActionData attackBattleActionData = battleRoleActionData.AttackBattleActionData;
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
            if (isCrash)
            {

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
