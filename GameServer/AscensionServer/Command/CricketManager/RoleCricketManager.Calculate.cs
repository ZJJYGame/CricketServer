using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
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
        public static CricketStatus CalculateStutas(CricketAptitude cricketAptitude, CricketPoint cricketPoint,CricketAddition cricketAddition)
        {
            //TODO补充技能的加成
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketStatusData>>(out var StatusDict);
            CricketStatus cricketStatus = new CricketStatus();

            cricketStatus.Atk = cricketAddition.Atk+ StatusDict[1].Atk+(int)((cricketAptitude.Str + cricketPoint.Str) * (cricketAptitude.StrAptitude + 100) * 0.01f);
            cricketStatus.Defense = cricketAddition.Defense + StatusDict[1].Defense+(int)((cricketAptitude.Def + cricketPoint.Def) * (cricketAptitude.DefAptitude + 100) * 0.01f);
            cricketStatus.Hp = cricketAddition.Hp + StatusDict[1].Hp + (int)((cricketAptitude.Con + cricketPoint.Con) * (cricketAptitude.ConAptitude + 100) * 0.01f);
            cricketStatus.Mp = cricketAddition.Mp + StatusDict[1].Mp + (int)(cricketStatus.Hp/200) +(cricketStatus.Mp);
            cricketStatus.MpReply = cricketAddition.MpReply + StatusDict[1].MpReply + (int)(cricketStatus.Mp /10)+ cricketStatus.MpReply;
            cricketStatus.Crt = (cricketAptitude.Dex+ cricketPoint.Dex)*(300-(2*(100- cricketAptitude.DexAptitude)))/1000000;
            cricketStatus.Eva = (cricketAptitude.Dex + cricketPoint.Dex) * (300 - (2 * (100 - cricketAptitude.DexAptitude))) / 1000000;
            cricketStatus.Speed = StatusDict[1].Speed + cricketStatus.Speed + (cricketAptitude.Dex*1.5f-(0.01*(100- cricketAptitude.DefAptitude)));
            cricketStatus.ReduceAtk = StatusDict[1].ReduceAtk ;
            cricketStatus.ReduceDef = StatusDict[1].ReduceDef;
            cricketStatus.Rebound = StatusDict[1].Rebound;

            return cricketStatus;
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
        public static void UpdateLevel(int cricketid, int exp,int roleid)
        {
            var nHCriteria = xRCommon.xRNHCriteria("CricketID", cricketid);
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteria);
            if (cricket!=null|| point!=null)
            {
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);
                cricket.Exp += exp;
                var tempcricket = new Cricket() { Exp= cricket .Exp,LevelID= cricket .LevelID};
                tempcricket= LevelUpCalculate(tempcricket);
                for (int i = cricket.LevelID+1; i <= tempcricket.LevelID; i++)
                {
                    point.FreePoint += cricketLevelDict[i].AssignPoint;
                }
                cricket.Exp = tempcricket.Exp;
                cricket.LevelID = tempcricket.LevelID;
                CricketDTO cricketDTO = new CricketDTO()
                {
                    ID = cricket.ID,
                    CricketID = cricket.CricketID,
                    CricketName = cricket.CricketName,
                    Exp = cricket.Exp,
                    LevelID = cricket.LevelID,
                    RankID = cricket.RankID,
                    SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict),
                    SpecialDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict)
                };

                var data = xRCommon.xRS2CParams();
                data.Add((byte)ParameterCode.Cricket, cricketDTO);
                data.Add((byte)ParameterCode.CricketPoint, point);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)CricketOperateType.LevelUp, Utility.Json.ToJson(data));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, dict);
                NHibernateQuerier.Update(cricket);
                NHibernateQuerier.Update(point);
            }
            else
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
            }
        }
        /// <summary>
        /// 学习技能
        /// </summary>
        public static void  StudySkill(int prop,int cricketid,int roleid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDict);
            var nHCriteriacricket= xRCommon.xRNHCriteria("ID", cricketid);
            var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);
            if (cricket != null)
            {
                if (propDict.ContainsKey(prop))
                {
                    #region 
                    var skills = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict);
                    if (skills.ContainsKey(propDict[prop].SkillID))
                    {
                        xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                        return;
                    }
                    else
                    {
                        skills.Add(propDict[prop].SkillID, 0);
                        //返回成功并更新数据库
                        cricket.SpecialDict = Utility.Json.ToJson(skills);
                        NHibernateQuerier.Update(cricket);
                        CricketDTO cricketDTO = new CricketDTO()
                        {
                            ID = cricket.ID,
                            CricketID = cricket.CricketID,
                            CricketName = cricket.CricketName,
                            Exp = cricket.Exp,
                            LevelID = cricket.LevelID,
                            RankID = cricket.RankID,
                            SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict),
                            SpecialDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict)
                        };
                       var data= xRCommon.xRS2CParams();
                        data.Add((byte)ParameterCode.Cricket, cricketDTO);
                        data.Add((byte)ParameterCode.Cricket, cricketstatus);
                        var dict = xRCommon.xRS2CSub();
                        dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                        xRCommon.xRS2CSend(roleid,(ushort)ATCmd.SyncCricket,(byte)ReturnCode .Success, dict);
                        return;
                    }
                    #endregion
                }
                else
                {
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    return;
                }
            }
            else
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                return;//返回失败
            }

        }
        /// <summary>
        /// 删除技能
        /// </summary>
        public static void RemoveSkill(int prop, int cricketid, int roleid)
        {
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);

            if (cricket!=null)
            {
                var skillList = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict);

                if (skillList.Count > 0)
                {
                    Random random = new Random();
                    var num = random.Next(0, skillList.Count);
                    skillList.Remove(num);
                    NHibernateQuerier.Update(cricket);
                    CricketDTO cricketDTO = new CricketDTO()
                    {
                        ID = cricket.ID,
                        CricketID = cricket.CricketID,
                        CricketName = cricket.CricketName,
                        Exp = cricket.Exp,
                        LevelID = cricket.LevelID,
                        RankID = cricket.RankID,
                        SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict),
                        SpecialDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict)
                    };
                    var data = xRCommon.xRS2CParams();
                    data.Add((byte)ParameterCode.Cricket, cricketDTO);
                    data.Add((byte)ParameterCode.Cricket, cricketstatus);
                    var dict = xRCommon.xRS2CSub();
                    dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Success, dict);
                    return;
                }
                else
                {
                    //返回失败
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    return;
                }
            }else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
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
        /// <summary>
        /// 计算加经验的
        /// </summary>
        /// <param name="cricket"></param>
        /// <returns></returns>
        public static Cricket  LevelUpCalculate(Cricket cricket)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);
            if (cricket .Exp>= cricketLevelDict[cricket.LevelID].ExpUP)
            {
                //Utility.Debug.LogInfo("YQZ蛐蛐等级" + cricket.LevelID + "蛐蛐经验" + cricket.Exp+"升级需要经验" + cricketLevelDict[cricket.LevelID].ExpUP);
                cricket.Exp -= cricketLevelDict[cricket.LevelID].ExpUP;
                cricket.LevelID+= 1;
                LevelUpCalculate(cricket);
            }
            return cricket;
        }

        public static void SetCricketValue()
        {

        }
    }
}
