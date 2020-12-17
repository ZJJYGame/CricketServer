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
                    }
                    else
                    {
                        xrDict[info.Key] = info.Value;
                    }
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId,  ExplorationItemDict = Utility.Json.ToJson(xrDict) });
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
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId, ExplorationItemDict = Utility.Json.ToJson(xrDict) });
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
                            var xrRandom = RandomManager(info.Key,setExploration[itemidInfo].Number[0], setExploration[itemidInfo].Number[1]);
                            switch (setExploration[itemidInfo].EventType)
                            {
                                case "AddStr":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddStr, AddNumber = xrRandom }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddCon":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddCon, AddNumber = xrRandom }, xrDict[info.Key].CustomId);
                                    break; 
                                case "AddDex":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddDex, AddNumber = xrRandom }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddDef":
                                    RoleCricketManager.AptitudeProp(roleId, new PropData() {  PropType = (int)RoleCricketManager.PropType.AddDef, AddNumber = xrRandom }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddAtk":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddAtk, AddNumber = xrRandom, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddHp":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddHp, AddNumber = xrRandom, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddDefense":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddDefense, AddNumber = xrRandom, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddMp":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddMp, AddNumber = xrRandom, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddMpReply":
                                    RoleCricketManager.StatusProp(roleId, new PropData() { PropType = (int)RoleCricketManager.PropType.AddMpReply, AddNumber = xrRandom, }, xrDict[info.Key].CustomId);
                                    break;
                                case "AddExp":
                                    RoleCricketManager.UpdateLevel(xrDict[info.Key].CustomId, xrRandom, roleId);
                                    break;
                                case "GetProp":
                                    Dictionary<int, ItemDTO> xrSet = new Dictionary<int, ItemDTO>();
                                    foreach (var infoProp in setExploration[itemidInfo].PropID)
                                        xrSet.Add(infoProp, new ItemDTO() { ItemAmount = 1 });
                                    InventoryManager.xRAddInventory(roleId, xrSet);
                                    break;
                                case "GetMoney":
                                    BuyPropManager.UpdateRoleAssets(roleId, xrRandom);
                                    break;
                                case "GetCricket"://全局id
                                    RoleCricketManager.AddCricket(xrDict[info.Key].GlobalId, roleId);
                                    break;
                                case "GetSkill":
                                    RoleCricketManager.AddSpecialSkill(setExploration[itemidInfo].SkillID,0,roleId);
                                    break;
                            }
                        }
                        xrDict.Remove(info.Key);
                        NHibernateQuerier.Update(new Exploration() { RoleID = roleId, ExplorationItemDict = Utility.Json.ToJson(xrDict) });
                    }
                }
                xRGetExploration(roleId);
            }
        }

        /// <summary>
        /// 针对战斗中的随机数
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
