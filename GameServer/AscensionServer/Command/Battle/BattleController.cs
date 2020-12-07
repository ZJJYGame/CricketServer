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
            //双方血量大于0一直执行
            while (playerOne.roleBattleData.Health > 0 && playerTwo.roleBattleData.Health > 0)
            {
                if (playerOne.remainActionBar < playerTwo.remainActionBar)
                {
                    attackPlayer = playerOne;
                    defendPlayer = playerTwo;
                }
                else
                {
                    attackPlayer = playerTwo;
                    defendPlayer = playerOne;
                }
                BattleSkill battleSkill = attackPlayer.RandomSkill();
                BattleDamageData battleDamageData = GetDamageData(battleSkill, attackPlayer, defendPlayer);
                //伤害结算
                defendPlayer.roleBattleData.OnHurt(battleDamageData);
                attackPlayer.roleBattleData.OnReboundHurt(battleDamageData);
                //耐力消耗
                attackPlayer.roleBattleData.OnEnduranceCost(battleSkill);
                //行动条结算
                attackPlayer.remainActionBar -= attackPlayer.roleBattleData.ActionBar;
                defendPlayer.remainActionBar -= attackPlayer.roleBattleData.ActionBar;
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
            int critDamage= (int)((defendPlayerData.ReceiveDamage / 100f) * ((atk+atk*attackPlayerData.CritDamage*(100-defendPlayerData.CritResistance)/100f) - defendPlayerData.Defence * (100 - attackPlayerData.Pierce) / 100f)) + battleSkill.DamageFixedValue;
            for (int i = 0; i < battleSkill.AttackNumber; i++)
            {
                Random dodgeRandom = new Random();
                int dodgeRandomIndex = dodgeRandom.Next(0, 101);
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
                    Random critRandom = new Random();
                    int critRandomIndex = critRandom.Next(0, 101);
                    if (critRandomIndex <= attackPlayerData.CritProp)//暴击了
                    {
                        battleDamageData.damageNumList.Add(critDamage);
                        battleDamageData.damageNumList.Add((int)(critDamage * defendPlayerData.ReboundDamage / 100f));
                        battleDamageData.isCritList.Add(true);
                    }
                    else//没暴击
                    {
                        battleDamageData.damageNumList.Add(normalDamage);
                        battleDamageData.damageNumList.Add((int)(normalDamage * defendPlayerData.ReboundDamage / 100f));
                        battleDamageData.isCritList.Add(false);
                    }
                }
            }
            return battleDamageData;
        }
    }
}
