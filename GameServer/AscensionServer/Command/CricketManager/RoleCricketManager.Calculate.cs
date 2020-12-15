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
            CricketAptitude cricketAptitude = new CricketAptitude();
            CricketStatus cricketStatus = new CricketStatus();
            CricketPoint cricketPoint = new CricketPoint();

     
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
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDict);

            if (propDict.ContainsKey(prop))
            {
                #region 
                var skills = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict);
                if (skills.ContainsKey(propDict[prop].SkillID))
                {
                    //返回失败
                    return;
                }
                else
                {
                    skills.Add(propDict[prop].SkillID, 1);
                    //返回成功并更新数据库
                    cricket.SpecialDict = Utility.Json.ToJson(skills);
                    NHibernateQuerier.Update(cricket);
                    return;
                }
                #endregion
            }

        }
        /// <summary>
        /// 删除技能
        /// </summary>
        public static void RemoveSkill( Cricket cricket)
        {
            var skillList = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict);

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
        /// <summary>
        /// 添加特殊技能
        /// </summary>
        /// <param name="skillid"></param>
        /// <param name="roleid"></param>
        public static void AddSpecialSkill(int skillid,int level,int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriaRole);

            var skills = Utility.Json.ToObject<Dictionary<int,int>>(cricket.SpecialDict);

            if (!skills.ContainsKey(skillid))
            {
                skills.Add(skillid, level);
            }
            else
            {
                skills[skillid] += level;
            }

            cricket.SpecialDict = Utility.Json.ToJson(skills);

            NHibernateQuerier.Update(cricket);
        }



    }
}
