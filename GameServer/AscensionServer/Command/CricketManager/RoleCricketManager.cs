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
        public static void GetRoleCricket(int roleid, CricketOperateType opType)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);
            Dictionary<int, CricketDTO> cricketsDict = new Dictionary<int, CricketDTO>();
            Dictionary<int, CricketStatus> statusDict = new Dictionary<int, CricketStatus>();
            Dictionary<int, CricketPoint> pointDict = new Dictionary<int, CricketPoint>();
            Dictionary<int, CricketAptitude> aptitudeDict = new Dictionary<int, CricketAptitude>();
            var dataDict = xRCommon.xRS2CParams();
            if (roleCricket != null)
            {
                var cricketDict = Utility.Json.ToObject<List<int>>(roleCricket.CricketList);
                Utility.Debug.LogInfo("yzqData获取蛐蛐属性:" + Utility.Json.ToJson(cricketDict));
                for (int i = 0; i < cricketDict.Count; i++)
                {
                    if (cricketDict[i] != -1)
                    {
                        var nHCriteriaCricket = xRCommon.xRNHCriteria("ID", cricketDict[i]);
                        var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketDict[i]);
                        var crickets = xRCommon.xRCriteria<Cricket>(nHCriteriaCricket);
                        CricketDTO cricketDTO = new CricketDTO()
                        {
                            ID = crickets.ID,
                            CricketID = crickets.CricketID,
                            CricketName = crickets.CricketName,
                            Exp = crickets.Exp,
                            LevelID = crickets.LevelID,
                            RankID = crickets.RankID,
                            SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(crickets.SkillDict),
                            SpecialDict = Utility.Json.ToObject<Dictionary<int, int>>(crickets.SpecialDict)
                        };
                        cricketsDict.Add(crickets.ID, cricketDTO);
                        statusDict.Add(crickets.ID, xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus));
                        pointDict.Add(crickets.ID, xRCommon.xRCriteria<CricketPoint>(nHCriteriastatus));
                        aptitudeDict.Add(crickets.ID, xRCommon.xRCriteria<CricketAptitude>(nHCriteriastatus));
                    }
                }
                dataDict.Add((byte)ParameterCode.RoleCricket, cricketDict);
                dataDict.Add((byte)ParameterCode.Cricket, cricketsDict);
                dataDict.Add((byte)ParameterCode.CricketStatus, statusDict);
                dataDict.Add((byte)ParameterCode.CricketPoint, pointDict);
                dataDict.Add((byte)ParameterCode.CricketAptitude, aptitudeDict);
                var messageDict = xRCommon.xRS2CSub();
                messageDict.Add((byte)opType, Utility.Json.ToJson(dataDict));
                Utility.Debug.LogInfo("yzqData发送所有蛐蛐属性:" + Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, messageDict);
                Utility.Debug.LogInfo("yzqData发送所有蛐蛐属性:" + Utility.Json.ToJson(dataDict)+"角色id"+ roleid);
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
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);
           
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);

            RoleCricketDTO roleCricketDTO = new RoleCricketDTO();
            roleCricketDTO.RoleID = roleCricket.RoleID;
            roleCricketDTO.CricketList = Utility.Json.ToObject<List<int>>(roleCricket.CricketList);
            roleCricketDTO.TemporaryCrickets = Utility.Json.ToObject<List<int>>(roleCricket.TemporaryCrickets);

            for (int i = 0; i < roleCricketDTO.TemporaryCrickets.Count; i++)
            {
                if (roleCricketDTO.TemporaryCrickets[i] != 0)
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
                    var cricketAddition = new CricketAddition();
                    cricketAddition.CricketID = cricket.ID;
                    NHibernateQuerier.Insert(cricketAddition);
                    var cricketPoint = new CricketPoint();
                    cricketPoint.FreePoint = cricketLevelDict[cricket.LevelID].AssignPoint;
                    cricketPoint.CricketID = cricket.ID;
                    NHibernateQuerier.Insert(cricketPoint);

                    roleCricketDTO.TemporaryCrickets[roleCricketDTO.TemporaryCrickets[i]] = cricket.ID;
                    break;
                }
            }

            roleCricket.CricketList = Utility.Json.ToJson(roleCricketDTO.TemporaryCrickets);
            NHibernateQuerier.Insert(roleCricket);
        }
        /// <summary>
        /// 放生小屋蟋蟀
        /// </summary>
        /// <param name="cricketid"></param>
        /// <param name="roleid"></param>
        public static void RemoveCricket( int roleid,int cricketid)
        {
            NHCriteria nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);

            var crickets = Utility.Json.ToObject<List<int>>(roleCricket.CricketList);
            if (crickets.Contains(cricketid) && cricketid != -1)
            {
                Utility.Debug.LogInfo("YZQData" + cricketid);
                NHCriteria nHCriteria = xRCommon.xRNHCriteria("ID", cricketid);
                var cricket = xRCommon.xRCriteria<Cricket>(nHCriteria);
                if (cricket != null)
                {
                    NHibernateQuerier.Delete(cricket);
                }
                NHCriteria nHCriteriaStatus = xRCommon.xRNHCriteria("CricketID", cricketid);

                var cricketStatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriaStatus);
                if (cricketStatus!=null)
                {
                    NHibernateQuerier.Delete(cricketStatus);
                }
                var cricketAptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteriaStatus);
                if (cricketAptitude!=null)
                {
                    NHibernateQuerier.Delete(cricketAptitude);
                }
                var cricketPoint = xRCommon.xRCriteria<CricketPoint>(nHCriteriaStatus);
                if (cricketPoint!=null)
                {
                    NHibernateQuerier.Delete(cricketPoint);
                }
                crickets.Remove(cricketid);
                crickets.Add(-1);
                roleCricket.CricketList = Utility.Json.ToJson(crickets);
                NHibernateQuerier.Update(roleCricket);

                GetRoleCricket(roleid,CricketOperateType.UpdCricket);

            }
        }
        /// <summary>
        /// 放生临时槽位
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="cricketid"></param>
        public static void RmvTempCricket(int roleid, int cricketid)
        {
            NHCriteria nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);

            var crickets = Utility.Json.ToObject<List<int>>(roleCricket.TemporaryCrickets);
            if (crickets.Contains(cricketid) && cricketid != -1)
            {
                Utility.Debug.LogInfo("YZQData" + cricketid);
                NHCriteria nHCriteria = xRCommon.xRNHCriteria("ID", cricketid);
                var cricket = xRCommon.xRCriteria<Cricket>(nHCriteria);
                if (cricket != null)
                {
                    NHibernateQuerier.Delete(cricket);
                }
                NHCriteria nHCriteriaStatus = xRCommon.xRNHCriteria("CricketID", cricketid);

                var cricketStatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriaStatus);
                if (cricketStatus != null)
                {
                    NHibernateQuerier.Delete(cricketStatus);
                }
                var cricketAptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteriaStatus);
                if (cricketAptitude != null)
                {
                    NHibernateQuerier.Delete(cricketAptitude);
                }
                var cricketPoint = xRCommon.xRCriteria<CricketPoint>(nHCriteriaStatus);
                if (cricketPoint != null)
                {
                    NHibernateQuerier.Delete(cricketPoint);
                }
                crickets.Remove(cricketid);
                crickets.Add(-1);
                roleCricket.CricketList = Utility.Json.ToJson(crickets);
                NHibernateQuerier.Update(roleCricket);

                GetRoleCricket(roleid, CricketOperateType.UpdTempCricket);

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
            var cricketStatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriaStatus);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriaCricket);
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
            var cricketPoint = xRCommon.xRCriteria<CricketPoint>(nHCriteria);
            if (cricketPoint!=null)
            {
                if ((cricketPointDTO.Dex + cricketPointDTO.Def + cricketPointDTO.Con + cricketPointDTO.Str) > cricketPoint.FreePoint)
                {
                    //返回加点失败
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
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
                    cricketPointDict.Add((byte)ParameterCode.CricketAptitude, cricketPoint);
                    cricketPointDict.Add((byte)ParameterCode.CricketStatus, cricketPoint);
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
        public static void GetTempCricket(int roleid,CricketOperateType opType)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);
            Dictionary<int, CricketDTO> cricketsDict = new Dictionary<int, CricketDTO>();
            Dictionary<int, CricketStatus> statusDict = new Dictionary<int, CricketStatus>();
            Dictionary<int, CricketAptitude> aptitudeDict = new Dictionary<int, CricketAptitude>();
            var dataDict = xRCommon.xRS2CParams();
            if (roleCricket != null)
            {
                var cricketDict = Utility.Json.ToObject<List<int>>(roleCricket.TemporaryCrickets);

                for (int i = 0; i < cricketDict.Count; i++)
                {
                    if (cricketDict[i] != -1)
                    {
                        var nHCriteriaCricket = xRCommon.xRNHCriteria("ID", cricketDict[i]);
                        var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketDict[i]);
                        var crickets = xRCommon.xRCriteria<Cricket>(nHCriteriaCricket);
                        CricketDTO cricketDTO = new CricketDTO()
                        {
                            ID = crickets.ID,
                            CricketID = crickets.CricketID,
                            CricketName = crickets.CricketName,
                            Exp = crickets.Exp,
                            LevelID = crickets.LevelID,
                            RankID = crickets.RankID,
                            SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(crickets.SkillDict)
                        };
                        cricketsDict.Add(cricketDict[i], cricketDTO);
                        statusDict.Add(cricketDict[i], xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus));
                        aptitudeDict.Add(cricketDict[i], xRCommon.xRCriteria<CricketAptitude>(nHCriteriastatus));
                    }
                }
                dataDict.Add((byte)ParameterCode.RoleCricket, cricketDict);
                dataDict.Add((byte)ParameterCode.Cricket, cricketsDict);
                dataDict.Add((byte)ParameterCode.CricketStatus, statusDict);
                dataDict.Add((byte)ParameterCode.CricketAptitude, aptitudeDict);
                var messageDict = xRCommon.xRS2CSub();
                messageDict.Add((byte)opType, Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, messageDict);
            }
        }
        /// <summary>
        /// 蟋蟀添加正常槽位
        /// </summary>
        public static void InsteadOfPos(int cricketid,int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleCricket = xRCommon.xRCriteria<RoleCricket>(nHCriteriaRole);
            if (roleCricket!=null)
            {
                var tempDict = Utility.Json.ToObject<List<int>>(roleCricket.TemporaryCrickets);
                var normalDict = Utility.Json.ToObject<List<int>>(roleCricket.CricketList);



                if (tempDict.Contains(cricketid) && cricketid != -1)
                {
                    for (int i = 0; i < normalDict.Count; i++)
                    {
                        if (normalDict[i] == -1)
                        {
                            normalDict[i] = cricketid;
                            tempDict.Remove(cricketid);
                            tempDict.Add(-1);
                            break;
                        }
                    }
                    roleCricket.CricketList = Utility.Json.ToJson(normalDict);
                    roleCricket.TemporaryCrickets = Utility.Json.ToJson(tempDict);
                    NHibernateQuerier.Update(roleCricket);
                    GetRoleCricket(roleid,CricketOperateType.UpdCricket);
                    GetTempCricket(roleid, CricketOperateType.UpdTempCricket);
                }
                else
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
            }
            else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_ReLogin);
        }
        /// <summary>
        /// 扩充蟋蟀窝
        /// </summary>
        /// <param name="roleid"></param>
        public static void EnlargeNest( int roleid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, NestLevel>>(out var nestLevelDict);
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleAssets = xRCommon.xRCriteria<RoleAssets>(nHCriteria);
            var roleCricket= xRCommon.xRCriteria<RoleCricket>(nHCriteria);
            if (roleAssets!=null)
            {
                if (roleCricket!=null)
                {
                    var crickets = Utility.Json.ToObject<List<int>>(roleCricket.CricketList);
                    if (roleAssets.RoleGold >= nestLevelDict[crickets.Count].Gold)
                    {
                        crickets.Add(-1);
                        roleCricket.CricketList = Utility.Json.ToJson(crickets);
                        roleAssets.RoleGold -= nestLevelDict[crickets.Count].Gold;
                        NHibernateQuerier.Update(roleCricket);
                        NHibernateQuerier.Update(roleAssets);
                        var dataDict = xRCommon.xRS2CParams();
                        dataDict.Add((byte)ParameterCode.RoleAsset, roleAssets);
                        dataDict.Add((byte)ParameterCode.RoleCricket, crickets);
                        var sendDict = xRCommon.xRS2CSub();
                        sendDict.Add((byte)CricketOperateType.EnlargeNest, Utility.Json.ToJson(dataDict));
                        xRCommon.xRS2CSend(roleid,(ushort)ATCmd.SyncCricket,(short)ReturnCode.Success, sendDict);
                    }
                    else
                        xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    return;
                }
                else
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                return;
            }
            else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
            return;
        }
    }
}
