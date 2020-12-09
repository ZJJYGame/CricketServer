﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol;

namespace AscensionServer
{
   public static  partial class RoleCricketManager
    {

        public static void GetRoleCricket(int roleid)
        {
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var roleCricket = NHibernateQuerier.CriteriaSelect<RoleCricket>(nHCriteriaRole);
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            Dictionary<int, CricketDTO> cricketsDict = new Dictionary<int, CricketDTO>();
            Dictionary<int, CricketStatus> statusDict = new Dictionary<int, CricketStatus>();
            Dictionary<int, CricketPoint> pointDict = new Dictionary<int, CricketPoint>();
            Dictionary<int, CricketAptitude> aptitudeDict = new Dictionary<int, CricketAptitude>();
            Dictionary<byte, string> dataDict = new Dictionary<byte, string>();
            if (roleCricket!=null)
            {
                var cricketDict = Utility.Json.ToObject<Dictionary<int,int>>(roleCricket.CricketList);

                foreach (var item in cricketDict)
                {
                    if (item.Value!=0)
                    {
                        NHCriteria nHCriteriaCricket = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Value);
                        NHCriteria nHCriteriastatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("CricketID", item.Value);
                        nHCriterias.Add(nHCriteriaCricket);
                        var crickets = NHibernateQuerier.CriteriaSelect<Cricket>(nHCriteriaCricket);
                        CricketDTO cricketDTO = new CricketDTO()
                        {
                            ID = crickets.ID,
                            CricketID = crickets.CricketID,
                            CricketName = crickets.CricketName,
                            Exp = crickets.Exp,
                            LevelID = crickets.LevelID,
                            RankID = crickets.RankID,
                            SkillList = Utility.Json.ToObject<List<int>>(crickets.SkillList)
                        };
                        cricketsDict.Add(item.Value, cricketDTO);
                        var cricketStatus = NHibernateQuerier.CriteriaSelect<CricketStatus>(nHCriteriastatus);
                        statusDict.Add(item.Value, cricketStatus);
                        var cricketPoint = NHibernateQuerier.CriteriaSelect<CricketPoint>(nHCriteriastatus);
                        pointDict.Add(item.Value, cricketPoint);
                        var cricketAptitude = NHibernateQuerier.CriteriaSelect<CricketAptitude>(nHCriteriastatus);
                        aptitudeDict.Add(item.Value, cricketAptitude);
                    }


                }

                dataDict.Add((byte)ParameterCode.Cricket,Utility.Json.ToJson(cricketsDict));
                dataDict.Add((byte)ParameterCode.CricketStatus, Utility.Json.ToJson(statusDict));
                dataDict.Add((byte)ParameterCode.CricketPoint, Utility.Json.ToJson(pointDict)); 
                dataDict.Add((byte)ParameterCode.CricketAptitude, Utility.Json.ToJson(aptitudeDict));
                Utility.Debug.LogInfo("yzqData请求蛐蛐属性发送了:" + Utility.Json.ToJson(dataDict));
                GameManager.CustomeModule<CricketManager>().S2CCricketMessage(roleid,Utility.Json.ToJson(dataDict),ReturnCode.Success);
                
            }

            GameManager.ReferencePoolManager.Despawns(nHCriterias);
        }


        /// <summary>
        /// 添加新蛐蛐
        /// </summary>
        /// <param name="cricketid"></param>
        /// <param name="roleid"></param>
        public static void AddCricket(int cricketid,int roleid)
        {

        }

        /// <summary>
        /// 获取单个蛐蛐的属性及等级
        /// </summary>
        /// <param name="cricketid"></param>
        /// <returns></returns>
        public static Dictionary<byte, object> GetCricketStatus(int cricketid)
        {
            NHCriteria nHCriteriaCricket = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", cricketid);
            NHCriteria nHCriteriaStatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("CricketID", cricketid);
            var cricketStatus = NHibernateQuerier.CriteriaSelect<CricketStatus>(nHCriteriaStatus);
            var cricket = NHibernateQuerier.CriteriaSelect<Cricket>(nHCriteriaCricket);
            Dictionary<byte, object> cricketData = new Dictionary<byte, object>();

            cricketData.Add((byte)ParameterCode.CricketStatus, cricketStatus);
            cricketData.Add((byte)ParameterCode.Cricket, cricket);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaCricket, nHCriteriaStatus);
            return cricketData;
        }

        /// <summary>
        /// 蛐蛐加点
        /// </summary>
        /// <param name="cricketid"></param>
        public static void AddPointForScricket(int roleid,int cricketid,CricketPointDTO cricketPointDTO)
        {
            NHCriteria nHCriteria = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("CricketID", cricketid);
            var cricketPoint = NHibernateQuerier.CriteriaSelect<CricketPoint>(nHCriteria);
            if ((cricketPointDTO.Dex + cricketPointDTO.Def + cricketPointDTO.Con + cricketPointDTO.Str)> cricketPoint.FreePoint)
            {
                //返回加点失败
                GameManager.CustomeModule<CricketManager>().S2CCricketMessage(roleid, "数据验证失败请重新登录", ReturnCode.Fail);
                return;
            }
            else
            {
                cricketPoint.Def += cricketPointDTO.Def;
                cricketPoint.Con += cricketPointDTO.Con;
                cricketPoint.Dex += cricketPointDTO.Dex;
                cricketPoint.Str += cricketPointDTO.Str;
                cricketPoint.FreePoint -= (cricketPointDTO.Dex + cricketPointDTO.Def + cricketPointDTO.Con + cricketPointDTO.Str);
                NHibernateQuerier.Update(cricketPoint);

                Dictionary<byte, string> dataDict = new Dictionary<byte, string>();
                Dictionary<byte, string> cricketPointDict = new Dictionary<byte, string>();
                cricketPointDict.Add((byte)ParameterCode.CricketPoint,Utility.Json.ToJson(cricketPoint));
                dataDict.Add((byte)CricketOperateType.AddPoint,Utility.Json.ToJson(cricketPointDict));
                GameManager.CustomeModule<CricketManager>().S2CCricketMessage(roleid, Utility.Json.ToJson(dataDict), ReturnCode.Success);

            }

        }



    }
}
