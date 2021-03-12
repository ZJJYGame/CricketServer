using AscensionProtocol;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;

namespace AscensionServer
{
    public partial class TaskManager
    {
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="roleId"></param>
        public async static void GetTask(int roleId)
        {
            Dictionary<int, TaskItemDTO> resultTaskItemDict;
            if (!await RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString()))
            {
                resultTaskItemDict = await RedisHelper.String.StringGetAsync<Dictionary<int, TaskItemDTO>>(RedisKeyDefine._DailyTaskPerfix);
                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString(), resultTaskItemDict);
            }
            else
            {
                resultTaskItemDict = await RedisHelper.Hash.HashGetAsync<Dictionary<int, TaskItemDTO>>(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString());
            }
            var pareams = xRCommon.xRS2CParams();
            pareams.Add((byte)ParameterCode.RoleTask, Utility.Json.ToJson(resultTaskItemDict));
            var subOp = xRCommon.xRS2CSub();
            subOp.Add((byte)subTaskOp.Get, pareams);
            xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncTask, (byte)ReturnCode.Success, subOp);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="roleId"></param>
        public static void AddTask(int roleId, Dictionary<int, TaskItemDTO> ItemInfo)
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
                GetTask(roleId);
            }
        }


        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public async static void UpdateTask(int roleId, Dictionary<int, TaskItemDTO> ItemInfo)
        {
            if (await RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString()))
            {
                Dictionary<int, TaskItemDTO> taskItemDict = await RedisHelper.Hash.HashGetAsync<Dictionary<int, TaskItemDTO>>(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString());
                foreach (var task in ItemInfo)
                {
                    if (!taskItemDict.ContainsKey(task.Key))
                        continue;
                    else
                    {
                        if (taskItemDict[task.Key].taskProgress >= taskItemDict[task.Key].taskTarget)
                            continue;
                        taskItemDict[task.Key].taskProgress += task.Value.taskProgress;
                    }
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString(), taskItemDict);
                }
                var pareams = xRCommon.xRS2CParams();
                pareams.Add((byte)ParameterCode.RoleTask, Utility.Json.ToJson(taskItemDict));
                var subOp = xRCommon.xRS2CSub();
                subOp.Add((byte)subTaskOp.Get, pareams);
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncTask, (byte)ReturnCode.Success, subOp);
            }
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public static void RemoveTask(int roleId, int taskId)
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
                GetTask(roleId);
            }
        }


        /// <summary>
        /// 领取任务奖励
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="ItemInfo"></param>
        public async static void VerifyTask(int roleId, Dictionary<int, TaskItemDTO> ItemInfo)
        {
            if (await RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString()))
            {
                Dictionary<int, TaskItemDTO> taskItemDict = await RedisHelper.Hash.HashGetAsync<Dictionary<int, TaskItemDTO>>(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString());
                foreach (var info in ItemInfo)
                {
                    if (!taskItemDict.ContainsKey(info.Key))
                        continue;
                    else
                    {
                        if (taskItemDict[info.Key].taskProgress >= taskItemDict[info.Key].taskTarget)
                        {
                            taskItemDict[info.Key].taskStatus = true;
                            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, TaskData>>(out var setTask);
                            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var setProp);
                            if (setTask[info.Key].PropID != 0)
                                ExplorationManager.xRAddExploration(roleId, new Dictionary<int, ExplorationItemDTO>(), new Dictionary<int, int> { { setTask[info.Key].PropID, 1 } });
                            BuyPropManager.UpdateRoleAssets(roleId, taskItemDict[info.Key].taskManoy);
                        }
                    }
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleDailyTaskRecordPerfix, roleId.ToString(), taskItemDict);
                }
                var pareams = xRCommon.xRS2CParams();
                pareams.Add((byte)ParameterCode.RoleTask, Utility.Json.ToJson(taskItemDict));
                var subOp = xRCommon.xRS2CSub();
                subOp.Add((byte)subTaskOp.Get, pareams);
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncTask, (byte)ReturnCode.Success, subOp);
            }
        }
    }
}
