using AscensionProtocol;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public partial class TaskManager
    {
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="roleId"></param>
        public static void xRGetTask(int roleId)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<xRTask>(nHcriteria);
                Utility.Debug.LogInfo("任务老陆==>" + xRserver.taskDict);
                var pareams = xRCommon.xRS2CParams();
                pareams.Add((byte)ParameterCode.RoleTask, xRserver.taskDict);
                var subOp = xRCommon.xRS2CSub();
                subOp.Add((byte)subTaskOp.Get, pareams);
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncTask, (byte)ReturnCode.Success, subOp);
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="roleId"></param>
        public static void xRAddTask(int roleId, Dictionary<int, TaskItemDTO> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<xRTask>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, TaskItemDTO>>(xRserver.taskDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                    {
                        xrDict[info.Key] = info.Value;
                    }
                    NHibernateQuerier.Update(new xRTask() { RoleID = roleId, taskDict = Utility.Json.ToJson(xrDict) });
                }
                xRGetTask(roleId);
            }
        }


        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRUpdateTask(int roleId, Dictionary<int, TaskItemDTO> ItemInfo)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<xRTask>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, TaskItemDTO>>(xRserver.taskDict);
                foreach (var info in ItemInfo)
                {
                    if (!xrDict.ContainsKey(info.Key))
                        continue;
                    else
                    {
                        if (xrDict[info.Key].taskProgress>=xrDict[info.Key].taskTarget)
                            continue;
                        xrDict[info.Key].taskProgress += info.Value.taskProgress;
                        if (xrDict[info.Key].taskProgress >= xrDict[info.Key].taskTarget)
                        {
                            xrDict[info.Key].taskStatus = true;
                            //xRRemove(roleId, info.Key);
                            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int,TaskData>>(out var setTask);
                            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var setProp);
                            if (setTask[info.Key].PropID != 0)
                                InventoryManager.xRUpdateInventory(roleId, new Dictionary<int, ItemDTO> { { setProp[setTask[info.Key].PropID].PropID, new ItemDTO() { ItemAmount = 1 } } });
                            BuyPropManager.UpdateRoleAssets(roleId, xrDict[info.Key].taskManoy);
                        }
                    }
                    NHibernateQuerier.Update(new xRTask() { RoleID = roleId,  taskDict = Utility.Json.ToJson(xrDict) });
                }
                xRGetTask(roleId);
            }
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void xRRemove(int roleId, int  taskId)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteria<xRTask>(nHcriteria);
                var xrDict = Utility.Json.ToObject<Dictionary<int, TaskItemDTO>>(xRserver.taskDict);
                if (xrDict.ContainsKey(taskId))
                {
                    xrDict.Remove(taskId);
                    NHibernateQuerier.Update(new xRTask() { RoleID = roleId, taskDict = Utility.Json.ToJson(xrDict) });
                }
                xRGetTask(roleId);
            }
        }
    }
}
