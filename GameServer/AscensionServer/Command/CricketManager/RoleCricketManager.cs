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
    public static partial class RoleCricketManager
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="roleid"></param>
        public static void GetRoleCricket(int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteriaSelectMethod<RoleCricket>(nHCriteriaRole);
            Dictionary<int, CricketDTO> cricketsDict = new Dictionary<int, CricketDTO>();
            Dictionary<int, CricketStatus> statusDict = new Dictionary<int, CricketStatus>();
            Dictionary<int, CricketPoint> pointDict = new Dictionary<int, CricketPoint>();
            Dictionary<int, CricketAptitude> aptitudeDict = new Dictionary<int, CricketAptitude>();
            var dataDict = xRCommon.xRS2CParams();
            if (roleCricket != null)
            {
                var cricketDict = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.CricketList);

                foreach (var item in cricketDict)
                {
                    if (item.Value != 0)
                    {
                        var nHCriteriaCricket = xRCommon.xRNHCriteria("ID", item.Value);
                        var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", item.Value);
                        var crickets = xRCommon.xRCriteriaSelectMethod<Cricket>(nHCriteriaCricket);
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
                        statusDict.Add(item.Value, xRCommon.xRCriteriaSelectMethod<CricketStatus>(nHCriteriastatus));
                        pointDict.Add(item.Value, xRCommon.xRCriteriaSelectMethod<CricketPoint>(nHCriteriastatus));
                        aptitudeDict.Add(item.Value, xRCommon.xRCriteriaSelectMethod<CricketAptitude>(nHCriteriastatus));
                    }
                }

                dataDict.Add((byte)ParameterCode.Cricket, cricketsDict);
                dataDict.Add((byte)ParameterCode.CricketStatus, statusDict);
                dataDict.Add((byte)ParameterCode.CricketPoint, pointDict);
                dataDict.Add((byte)ParameterCode.CricketAptitude, aptitudeDict);
                var messageDict = xRCommon.xRS2CSub();
                messageDict.Add((byte)CricketOperateType.GetCricket, Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, messageDict);
            }
        }

        /// <summary>
        /// 添加新蛐蛐
        /// </summary>
        /// <param name="cricketid"></param>
        /// <param name="roleid"></param>
        public static void AddCricket(int cricketid, int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteriaSelectMethod<RoleCricket>(nHCriteriaRole);
           
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);

            RoleCricketDTO roleCricketDTO = new RoleCricketDTO();
            roleCricketDTO.RoleID = roleCricket.RoleID;
            roleCricketDTO.CricketList = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.CricketList);
            roleCricketDTO.TemporaryCrickets = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.TemporaryCrickets);

            foreach (var item in roleCricketDTO.TemporaryCrickets)
            {
                if (item.Value != 0)
                {
                    var cricketStatus = new CricketStatus();
                    var cricketAptitude = new CricketAptitude();
                    cricketAptitude.ConAptitude = RandomNum(1, 101);
                    cricketAptitude.StrAptitude = RandomNum(1, 101);
                    cricketAptitude.DefAptitude = RandomNum(1, 101);
                    cricketAptitude.DexAptitude = RandomNum(1, 101);
                    var cricket = new Cricket();
                    cricket = NHibernateQuerier.Insert(cricket);
                    cricketStatus.CricketID = cricket.ID;
                    NHibernateQuerier.Insert(cricketStatus);
                    cricketAptitude.CricketID = cricket.ID;
                    NHibernateQuerier.Insert(cricketAptitude);
                    var cricketPoint = new CricketPoint();
                    cricketPoint.FreePoint = cricketLevelDict[cricket.LevelID].AssignPoint;
                    cricketPoint.CricketID = cricket.ID;
                    NHibernateQuerier.Insert(cricketPoint);

                    roleCricketDTO.TemporaryCrickets[item.Key] = cricket.ID;
                    break;
                }
            }

            roleCricket.CricketList = Utility.Json.ToJson(roleCricketDTO.TemporaryCrickets);
            NHibernateQuerier.Insert(roleCricket);
        }
        /// <summary>
        /// 放生蟋蟀
        /// </summary>
        /// <param name="cricketid"></param>
        /// <param name="roleid"></param>
        public static void RemoveCricket( int roleid,RoleCricketDTO roleCricketDTO)
        {
            NHCriteria nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteriaSelectMethod<RoleCricket>(nHCriteriaRole);

            //NHCriteria nHCriteria = xRCommon.xRNHCriteria("ID", cricketid); ;
            //NHCriteria nHCriteriaStatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var tempCricket = roleCricketDTO.TemporaryCrickets;
            var crickets = roleCricketDTO.CricketList;
            if (tempCricket.Count>0)
            {

            }
        }

        /// <summary>
        /// 获取单个蛐蛐的属性及等级
        /// </summary>
        /// <param name="cricketid"></param>
        /// <returns></returns>
        public static Dictionary<byte, object> GetCricketStatus(int cricketid)
        {
            NHCriteria nHCriteriaCricket = xRCommon.xRNHCriteria("ID", cricketid);
            NHCriteria nHCriteriaStatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricketStatus = xRCommon.xRCriteriaSelectMethod<CricketStatus>(nHCriteriaStatus);
            var cricket = xRCommon.xRCriteriaSelectMethod<Cricket>(nHCriteriaCricket);
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
        public static void AddPointForScricket(int roleid, int cricketid, CricketPointDTO cricketPointDTO)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricketPoint = xRCommon.xRCriteriaSelectMethod<CricketPoint>(nHCriteria);
            if (cricketPoint!=null)
            {
                if ((cricketPointDTO.Dex + cricketPointDTO.Def + cricketPointDTO.Con + cricketPointDTO.Str) > cricketPoint.FreePoint)
                {
                    //返回加点失败
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
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

                    var dataDict = xRCommon.xRS2CSub();
                    var cricketPointDict = xRCommon.xRS2CParams();
                    cricketPointDict.Add((byte)ParameterCode.CricketPoint, cricketPoint);
                    dataDict.Add((byte)CricketOperateType.AddPoint, Utility.Json.ToJson(cricketPointDict));
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, dataDict);

                }
            }
            else
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
            }

        }
        /// <summary>
        /// 获取临时槽位蟋蟀
        /// </summary>
        /// <param name="roleid"></param>
        public static void GetTempCricket(int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteriaSelectMethod<RoleCricket>(nHCriteriaRole);
            Dictionary<int, CricketDTO> cricketsDict = new Dictionary<int, CricketDTO>();
            Dictionary<int, CricketStatus> statusDict = new Dictionary<int, CricketStatus>();
            Dictionary<int, CricketAptitude> aptitudeDict = new Dictionary<int, CricketAptitude>();
            var dataDict = xRCommon.xRS2CParams();
            if (roleCricket != null)
            {
                var cricketDict = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.TemporaryCrickets);

                foreach (var item in cricketDict)
                {
                    if (item.Value != 0)
                    {
                        var nHCriteriaCricket = xRCommon.xRNHCriteria("ID", item.Value);
                        var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", item.Value);
                        var crickets = xRCommon.xRCriteriaSelectMethod<Cricket>(nHCriteriaCricket);
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
                        statusDict.Add(item.Value, xRCommon.xRCriteriaSelectMethod<CricketStatus>(nHCriteriastatus));
                        aptitudeDict.Add(item.Value, xRCommon.xRCriteriaSelectMethod<CricketAptitude>(nHCriteriastatus));
                    }
                }

                dataDict.Add((byte)ParameterCode.Cricket, cricketsDict);
                dataDict.Add((byte)ParameterCode.CricketStatus, statusDict);
                dataDict.Add((byte)ParameterCode.CricketAptitude, aptitudeDict);
                var messageDict = xRCommon.xRS2CSub();
                messageDict.Add((byte)CricketOperateType.GetTempCricket, Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, messageDict);
            }
        }
        /// <summary>
        /// 蟋蟀添加正常槽位
        /// </summary>
        public static void InsteadOfPos(int index,int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteriaSelectMethod<RoleCricket>(nHCriteriaRole);
            if (roleCricket!=null)
            {
                var tempDict = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.TemporaryCrickets);
                var normalDict = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.CricketList);

                tempDict.TryGetValue(index, out int cricketid);

                if (cricketid!=0)
                {
                    foreach (var item in normalDict)
                    {
                        if (item.Value==0)
                        {
                            normalDict[item.Key] = cricketid;
                            tempDict[index] =0;
                            break;
                        }
                    }
                    roleCricket.CricketList = Utility.Json.ToJson(normalDict);
                    roleCricket.TemporaryCrickets = Utility.Json.ToJson(tempDict);
                    NHibernateQuerier.Update(roleCricket);
                    GetRoleCricket(roleid);
                    GetTempCricket(roleid);
                }
                else
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
            }
            else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
        }
    }
}
