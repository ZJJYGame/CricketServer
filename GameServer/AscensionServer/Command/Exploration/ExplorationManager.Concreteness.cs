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
                        xrDict[info.Key] = info.Value;
                    }
                    NHibernateQuerier.Update(new Exploration() { RoleID = roleId, ExplorationItemDict = Utility.Json.ToJson(xrDict) });
                }
                xRGetExploration(roleId);
            }
        }


    }
}
