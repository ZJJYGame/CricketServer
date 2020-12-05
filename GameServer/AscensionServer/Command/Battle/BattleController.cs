using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            }
        }
    }
}
