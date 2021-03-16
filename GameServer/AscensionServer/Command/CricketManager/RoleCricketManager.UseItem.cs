using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using Protocol;
namespace AscensionServer
{
    public static partial class RoleCricketManager
    {

        public enum PropType
        {
            AddExp = 1,
            AddStr = 2,
            AddCon = 3,
            AddDex = 4,
            AddDef = 5,
            AddAtk = 6,
            AddDefense = 7,
            AddHp = 8,
            AddMp = 9,
            AddMpReply = 10,
            Skill = 11,
            DeleteSkill = 12,
            Reset=13,
        }

        public static void DifferentiateGlobal(int propid,int roleid,int cricketid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDataDict);

            var result = propDataDict.TryGetValue(propid, out var propData);
            if (result)
            {
                switch ((PropType)propData.PropType)
                {
                    case PropType.AddExp:
                        ConsumeProp(cricketid, propData, roleid);
                        break;
                    case PropType.AddStr:
                        AptitudeProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddCon:
                        AptitudeProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddDef:
                        AptitudeProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddDex:
                        AptitudeProp(roleid, propData, cricketid);
                        break;
                    case PropType.DeleteSkill:
                        ConsumeProp(cricketid, propData, roleid);
                        break;
                    case PropType.AddAtk:
                        StatusProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddDefense:
                        StatusProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddHp:
                        StatusProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddMp:
                        StatusProp(roleid, propData, cricketid);
                        break;
                    case PropType.AddMpReply:
                        StatusProp(roleid, propData, cricketid);
                        break;
                    case PropType.Skill:
                        ConsumeProp(cricketid, propData, roleid);
                        break;
                    case PropType.Reset:
                        ConsumeProp(cricketid, propData, roleid);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 区分加成物品
        /// </summary>
        public static void  AptitudeProp(int roleid,PropData propData,int cricketid)
        {
            var nHCriteriaAptitude = xRCommon.xRNHCriteria("CricketID", cricketid);
            var aptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteriaAptitude);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteriaAptitude);
            var addition = xRCommon.xRCriteria<CricketAddition>(nHCriteriaAptitude);
            var cricketObj = xRCommon.xRCriteria<Cricket>(nHCriteriaAptitude);
            if (point != null && aptitude != null&& addition!= null && cricketObj != null)
            {
                switch ((PropType)propData.PropType)
                {
                    case PropType.AddStr:
                        aptitude.Str += propData.AddNumber;
                        break;
                    case PropType.AddCon:
                        aptitude.Con += propData.AddNumber;
                        break;
                    case PropType.AddDef:
                        aptitude.Def += propData.AddNumber;
                        break;
                    case PropType.AddDex:
                        aptitude.Dex += propData.AddNumber;
                        break;
                    default:
                        break;
                }
                //var status = CalculateStutas(aptitude, point, addition);
                var status = SkillAdditionStatus(cricketObj, aptitude, point, addition);
                status.CricketID = aptitude.CricketID;
                var data = xRCommon.xRS2CParams();
                data.Add((byte)ParameterCode.CricketStatus, status);
                data.Add((byte)ParameterCode.CricketAptitude, aptitude);
                data.Add((byte)ParameterCode.CricketPoint, point);
                NHibernateQuerier.Update(status);
                var dict= xRCommon.xRS2CSub();
                dict.Add((byte)CricketOperateType.AddPoint, Utility.Json.ToJson(data));
                xRCommon.xRS2CSend(roleid,(ushort)ATCmd.SyncCricket,(short)ReturnCode.Success, dict);
                //更新背包
                InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { propData.PropID, new ItemDTO() { ItemAmount = 1 } } });
                //TODO更新数据库并发送
                NHibernateQuerier.Update(aptitude);
                NHibernateQuerier.Update(status);
            }
            else
            {
                //返回失败
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                return;
            }
        }
        /// <summary>
        /// 区分属性加成
        /// </summary>
        /// <param name="propData"></param>
        /// <param name="cricketStatus"></param>
        public static void StatusProp(int roleid, PropData propData, int cricketid)
        {

            var nHCriteriaAptitude = xRCommon.xRNHCriteria("CricketID", cricketid);
            var aptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteriaAptitude);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteriaAptitude);
            var addition = xRCommon.xRCriteria<CricketAddition>(nHCriteriaAptitude);
            var cricketObj = xRCommon.xRCriteria<Cricket>(nHCriteriaAptitude);
            if (point != null && addition != null && aptitude != null && cricketObj != null)
            {
                switch ((PropType)propData.PropType)
                {
                    case PropType.AddAtk:
                        addition.Atk += propData.AddNumber;
                        break;
                    case PropType.AddDefense:
                        addition.Defense += propData.AddNumber;
                        break;
                    case PropType.AddHp:
                        addition.Hp += propData.AddNumber;
                        break;
                    case PropType.AddMp:
                        addition.Mp += propData.AddNumber;
                        break;
                    case PropType.AddMpReply:
                        addition.MpReply += propData.AddNumber;
                        break;
                    default:
                        break;
                }
            }
            //var status = CalculateStutas(aptitude, point, addition);
            var status = SkillAdditionStatus(cricketObj, aptitude, point, addition);
            status.CricketID = aptitude.CricketID;
            var data = xRCommon.xRS2CParams();
            data.Add((byte)ParameterCode.CricketStatus, status);
            data.Add((byte)ParameterCode.CricketAptitude, aptitude);
            data.Add((byte)ParameterCode.CricketPoint, point);
            NHibernateQuerier.Update(status);
            var dict = xRCommon.xRS2CSub();
            dict.Add((byte)CricketOperateType.AddPoint, Utility.Json.ToJson(data));
            xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, dict);
            //更新背包
            InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { propData.PropID, new ItemDTO() { ItemAmount = 1 } } });
            //TODO更新数据库并发送
            NHibernateQuerier.Update(addition);
            NHibernateQuerier.Update(status);
        }
        /// <summary>
        /// 区分消耗物品
        /// </summary>
        public static void ConsumeProp(int cricketid,PropData propData,int roleid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDataDict);
            //是否存在该物品
            if (!propDataDict.ContainsKey(propData.PropID))
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                return;
            }
            switch ((PropType)propData.PropType)
            {
                case PropType.AddExp:
                    Utility.Debug.LogInfo("YZQ增加经验的蛐蛐"+ cricketid);
                        UpdateLevel(cricketid,propData,roleid);             
                    break;         
                case PropType.DeleteSkill:
                    Utility.Debug.LogInfo("YZQ删除技能的蛐蛐" + cricketid);
                    RemoveSkill(propData.PropID, cricketid, roleid);
                    break;              
                case PropType.Skill:
                    Utility.Debug.LogInfo("YZQ学习技能的蛐蛐" + cricketid);
                    StudySkill(propData.PropID, cricketid,roleid);
                    break;
                case PropType.Reset:
                    Utility.Debug.LogInfo("YZQ重置加点的蛐蛐" + cricketid);
                    ReSetPoint(roleid,cricketid, propData.PropID);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 验证物品及类型是否存在
        /// </summary>
        /// <param name="propid"></param>
        /// <param name="propType"></param>
        /// <param name="propData"></param>
        /// <returns></returns>
        public static bool VerifyProp(int propid,PropType propType,out PropData propData)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var cricketLevelDict);
            var result = cricketLevelDict.TryGetValue(propid, out  propData);
            if (result)
            {
                if (propData.PropType == (int)propType)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}
