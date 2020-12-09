using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

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

                //行动条结算
                attackPlayer.ChangeActionBar(attackPlayer.roleBattleData.ActionBar);
                defendPlayer.ChangeActionBar(attackPlayer.roleBattleData.ActionBar);
                //todo所有buff实体持续时间减少
                attackPlayer.battleBuffController.UpdateBuffTime(attackPlayer.roleBattleData.ActionBar);
                defendPlayer.battleBuffController.UpdateBuffTime(attackPlayer.roleBattleData.ActionBar);

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

                //Utility.Debug.LogInfo(attackPlayer.RoleId + "的行动条为" + attackPlayer.RemainActionBar + "," + defendPlayer.RoleId+"的行动条为" + defendPlayer.RemainActionBar);
            }
            Utility.Debug.LogInfo("战斗计算流程结束");
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
        public void InitController(BattleCharacterEntity battleCharacterEntityOne,BattleCharacterEntity battleCharacterEntityTwo)
        {
            playerOne = battleCharacterEntityOne;
            playerTwo = battleCharacterEntityTwo;
        }
    }
}
