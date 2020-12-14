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
        /// <summary>
        /// 获取资质等随机随机
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="num"></param>
        public static int RandomNum(int min,int max)
        {
            int num = 50;
            Random random = new Random();
            num= random.Next(min,max);
            return num;
        }
        /// <summary>
        /// 加经验升级
        /// </summary>
        public static Cricket UpdateLevel(Cricket cricket,int exp)
        {
            int addtion = cricket.Exp + exp;
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);

            if (addtion> cricketLevelDict[cricket.LevelID].ExpUP)
            {
                cricket.Exp = addtion - cricketLevelDict[cricket.LevelID].ExpUP;
                cricket.LevelID += 1;
            }
            else
            {
                cricket.Exp = addtion;
            }

            return cricket;
        }
        /// <summary>
        /// 学习技能
        /// </summary>
        public static void  StudySkill(int prop,Cricket cricket)
        {
            var skillList = Utility.Json.ToObject<List<int>>(cricket.SkillList);
            if (skillList.Contains(prop))
            {
                //返回失败
                return;
            }
            else
            {
                skillList.Add(prop);
                //返回成功并更新数据库
                return;
            }
        }
        /// <summary>
        /// 删除技能
        /// </summary>
        public static void RemoveSkill( Cricket cricket)
        {
            var skillList = Utility.Json.ToObject<List<int>>(cricket.SkillList);

            if (skillList.Count>0)
            {
                Random random = new Random();
               var num= random.Next(0, skillList.Count);
                skillList.Remove(num);
                //返回成功并更新数据库
                return;
            }
            else
            {
                //返回失败
                return;
            }
        }

    }
}
