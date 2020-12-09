using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
  public static partial  class RoleCricketManager
    {
        /// <summary>
        /// 重置蟋蟀加点
        /// </summary>
        /// <param name="level">当前蟋蟀等级</param>
        /// <returns></returns>
        public static int  ReSetPoint(int level)
        {
            int points = 0;
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int,  CricketLevel>>(out var cricketLevelDict);
            for (int i = 1; i <=level; i++)
            {
                points += cricketLevelDict[i].AssignPoint;
            }
            return points;
        }
        /// <summary>
        /// 计算属性加成
        /// </summary>
        public static void CalculateStutas()
        {

        }




    }
}
