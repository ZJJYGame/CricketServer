using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
    public partial class ExplorationManager
    {
        /// <summary>
        /// 获取探索
        /// </summary>
        /// <param name="roleId"></param>
        public static void xRGetExploration(int roleId)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<Exploration>(nHcriteria);
                Utility.Debug.LogInfo("老陆==>" + xRserver.ExplorationItemDict);
                var pareams = xRCommon.xRS2CParams();
                pareams.Add((byte)ParameterCode.RoleExploration, xRserver.ExplorationItemDict);
                pareams.Add((byte)ParameterCode.RoleExplorationUnlock, xRserver.UnLockDict);
                var subOp = xRCommon.xRS2CSub();
                subOp.Add((byte)SubOperationCode.Get, pareams);
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncExploration, (byte)ReturnCode.Success, subOp);
            }
        }

        /// <summary>
        /// 添加探索
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRAddExploration (int roleId, Dictionary<int, ExplorationItemDTO> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<Exploration>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, ExplorationItemDTO>>(xRserver.ExplorationItemDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                    {
                        xrDict[info.Key] = info.Value;
                        GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ExplorationData>>(out var setExploration);
                        for (int propInfo = 0; propInfo < xrDict[info.Key].ItemId.Count; propInfo++)
                        {

                            int xrRandom = 1;
                            if (setExploration[xrDict[info.Key].ItemId.ToList()[propInfo].Key].Number.Count == 2)
                            {

                                xrRandom = RandomManager(info.Key, setExploration[xrDict[info.Key].ItemId.ToList()[propInfo].Key].Number[0], setExploration[xrDict[info.Key].ItemId.ToList()[propInfo].Key].Number[1]);
                            }
                            xrDict[info.Key].ItemId[xrDict[info.Key].ItemId.ToList()[propInfo].Key] = xrRandom;
                            switch (setExploration[xrDict[info.Key].ItemId.ToList()[propInfo].Key].EventType)
                            {
                                case "GetProp":
                                case "GetCricket"://全局id
                                case "GetSkill":
                                    xrDict[info.Key].ItemId[xrDict[info.Key].ItemId.ToList()[propInfo].Key] = 1;
                                    break;
                                case "AddExp":
                                    var nHcriteriaID = xRCommon.xRNHCriteria("ID", info.Value.CustomId);
                                    var xRserverGrade = xRCommon.xRCriteria<Cricket>(nHcriteriaID);
                                    var gradeValue = info.Key == 0 ? 100 : info.Key == 1 ? 400 : info.Key == 2 ? 800 : 1600;
                                    var percentValue = setExploration[xrDict[info.Key].ItemId.ToList()[propInfo].Key].Number[0];
                                    var levbelValue = xRserverGrade.LevelID* xRserverGrade.LevelID;
                                    var expValue = info.Key == 0 ? levbelValue*6*percentValue: info.Key == 1 ? levbelValue*12 : info.Key == 2 ? levbelValue*18 : levbelValue*24;
                                    xrDict[info.Key].ItemId[xrDict[info.Key].ItemId.ToList()[propInfo].Key] = expValue / 100 < gradeValue ? gradeValue : expValue / 100;
                                    break;
                            }
                        }
                    }
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId,  ExplorationItemDict = Utility.Json.ToJson(xrDict), UnLockDict = xRserver.UnLockDict });
                }
                xRGetExploration(roleId);
            }
        }


        /// <summary>
        /// 更新探索
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRUpdateExploration(int roleId, Dictionary<int, ExplorationItemDTO> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<Exploration>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, ExplorationItemDTO>>(xRserver.ExplorationItemDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                        return;
                    else
                    {
                        xrDict[info.Key].TimeType -= info.Value.TimeType;
                    }
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId, ExplorationItemDict = Utility.Json.ToJson(xrDict),  UnLockDict = xRserver.UnLockDict });
                }
                xRGetExploration(roleId);
            }
        }
        /// <summary>
        /// 移除探索
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRRemoveExploration(int roleId, Dictionary<int, ExplorationItemDTO> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<Exploration>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, ExplorationItemDTO>>(xRserver.ExplorationItemDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                        return;
                    else
                    {
                        GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ExplorationData>>(out var setExploration);
                        //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleSkill>>(out var setExploration);
                        foreach (var itemidInfo in xrDict[info.Key].ItemId)
                        {
                            switch (setExploration[itemidInfo.Key].EventType)
                            {
                                case "AddStr":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddStr, AddNumber = itemidInfo.Value }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddCon":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddCon, AddNumber = itemidInfo.Value }, xrDict[info.Key].CustomId);
                                    break; 
                                case "AddDex":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddDex, AddNumber = itemidInfo.Value }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddDef":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() {  PropType = (int)RoleCricketManager.PropType.AddDef, AddNumber = itemidInfo.Value }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddAtk":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddAtk, AddNumber = itemidInfo.Value, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddHp":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddHp, AddNumber = itemidInfo.Value, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddDefense":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddDefense, AddNumber = itemidInfo.Value, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddMp":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddMp, AddNumber = itemidInfo.Value, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddMpReply":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddMpReply, AddNumber = itemidInfo.Value, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddExp":
                                    RoleCricketManager.UpdateLevel(xrDict[info.Key].CustomId, new PropData() { PropID =-1, AddNumber = itemidInfo.Value, }, roleId);
                                    break;
                                case "GetProp":
                                    Dictionary<int, ItemDTO> xrSet = new Dictionary<int, ItemDTO>();
                                    foreach (var infoProp in setExploration[itemidInfo.Key].PropID)
                                        xrSet.Add(infoProp, new ItemDTO() { ItemAmount = itemidInfo.Value });
                                    InventoryManager.xRAddInventory(roleId, xrSet);
                                    break;
                                case "GetMoney":
                                    BuyPropManager.UpdateRoleAssets(roleId, itemidInfo.Value);
                                    break;
                                case "GetCricket"://全局id
                                    RoleCricketManager.AddCricket(xrDict[info.Key].GlobalId, roleId);
                                    break;
                                case "GetSkill":
                                    RoleCricketManager.AddSpecialSkill(setExploration[itemidInfo.Key].SkillID,0,roleId, xrDict[info.Key].CustomId);
                                    break;
                            }
                        }
                        xrDict.Remove(info.Key);
                        NHibernateQuerier.Update(new Exploration() { RoleID = roleId, ExplorationItemDict = Utility.Json.ToJson(xrDict), UnLockDict =xRserver.UnLockDict });
                    }
                }
                xRGetExploration(roleId);
            }
        }


        /// <summary>
        /// 验证是否解锁
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRVerifyExploration(int roleId, Dictionary<int, bool> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<Exploration>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, bool>>(xRserver.UnLockDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                    {
                        xrDict[info.Key] = true;
                        xrDict[info.Key+1] = false;
                    }
                    else
                    {
                        xrDict[info.Key] = true;
                        xrDict[info.Key + 1] = false;
                    }
                    BuyPropManager.ExpenseRoleAssets(roleId, 1000);
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId,  ExplorationItemDict = xRserver.ExplorationItemDict,  UnLockDict = Utility.Json.ToJson(xrDict) });
                }
                xRGetExploration(roleId);
            }
        }





        /// <summary>
        /// 针对服务器中的随机数
        /// </summary>
        /// <param name="ov"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int RandomManager(int ov, int minValue, int maxValue)
        {
            var targetValue = new Random((int)DateTime.Now.Ticks + ov).Next(minValue, maxValue);
            return targetValue;
        }

    }
}
