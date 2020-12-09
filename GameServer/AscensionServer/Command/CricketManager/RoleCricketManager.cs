using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol;

namespace AscensionServer
{
   public static class RoleCricketManager
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
    }
}
