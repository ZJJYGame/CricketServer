using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    [CustomeModule]
    public partial class TaskManager : Module<TaskManager>
    {
        Dictionary<int, TaskItemDTO> darilyDict = new Dictionary<int, TaskItemDTO>();
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncTask, C2STask);

        private void C2STask(OperationData opData)
        {
            Utility.Debug.LogInfo("老陆==>" + (opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, TaskDTO>>(data.Values.ToList()[0].ToString());
            switch ((subTaskOp)data.Keys.ToList()[0])
            {
                case subTaskOp.Get:
                    if (darilyDict.Count != 3)
                        SetTaskAtFixedTime(roleSet[(byte)ParameterCode.RoleTask].RoleID);
                    xRGetTask(roleSet[(byte)ParameterCode.RoleTask].RoleID);
                    break;
                case subTaskOp.Add:
                    xRAddTask(roleSet[(byte)ParameterCode.RoleTask].RoleID, roleSet[(byte)ParameterCode.RoleTask].taskDict);
                    break;
                case subTaskOp.Update:
                    xRUpdateTask(roleSet[(byte)ParameterCode.RoleTask].RoleID, roleSet[(byte)ParameterCode.RoleTask].taskDict);
                    break;
                case subTaskOp.Remove:
                    xRRemoveTask(roleSet[(byte)ParameterCode.RoleTask].RoleID, roleSet[(byte)ParameterCode.RoleTask].taskDict.ToList()[0].Key);
                    break;
                case subTaskOp.Verify:
                    xRVerifyTask(roleSet[(byte)ParameterCode.RoleTask].RoleID, roleSet[(byte)ParameterCode.RoleTask].taskDict);
                    break;
            }
        }
        public override void OnRefresh()
        {
            base.OnRefresh();
            DateTime now = DateTime.Now;
            if (now.Hour == 16 && (now.Minute == 45 && now.Second == 59))
                darilyDict.Clear();
        }
        private void SetTaskAtFixedTime(int roleId)
        {
            DateTime now = DateTime.Now;
            DateTime oneClock = DateTime.Today.AddHours(16.43);//凌晨一点 
            if (now > oneClock)
                TimeSetTask(roleId);
        }

        public void TimeSetTask(int roleId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, TaskData>>(out var setTask);
            int index = 0;
            while (darilyDict.Count != 3)
            {
                if (darilyDict.Count == 3)
                    break;
                var xrtaskId = ExplorationManager.RandomManager(index++, 0, setTask.Count);
                if (!darilyDict.ContainsKey(setTask.Keys.ToList()[xrtaskId]))
                {
                    darilyDict[setTask.Keys.ToList()[xrtaskId]] = new TaskItemDTO() { taskStatus = false, taskProgress = 0, taskTarget = setTask.Values.ToList()[xrtaskId].TaskTarget, taskManoy = 100 };
                }
            }
            NHibernateQuerier.Update(new xRTask() { RoleID = roleId, taskDict = Utility.Json.ToJson(darilyDict) });
        }

        /// <summary>
        /// 获取多个随机的任务数据
        /// </summary>
        /// <param name="count">获取的数量</param>
        /// <returns></returns>
        public Dictionary<int, TaskItemDTO> GetRandomTask(int count)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, TaskData>>(out var taskJsonDict);
            List<TaskData> taskDataJsonList = taskJsonDict.Values.ToList();
            List<TaskData> resultTaskDataList = new List<TaskData>();
            for (int i = 0; i < count; i++)
            {
                if (taskDataJsonList.Count == 0)
                    break;
                resultTaskDataList.Add(taskDataJsonList[Utility.Algorithm.CreateRandomInt(0, taskDataJsonList.Count)]);
            }

            Dictionary<int, TaskItemDTO> taskItemDIct = new Dictionary<int, TaskItemDTO>();
            for (int i = 0; i < resultTaskDataList.Count; i++)
            {
                taskItemDIct[resultTaskDataList[i].TaskId] = new TaskItemDTO()
                {
                    taskTarget = resultTaskDataList[i].TaskTarget,
                    taskProgress = 0,
                    taskStatus = false,
                    taskManoy = Utility.Algorithm.CreateRandomInt(resultTaskDataList[i].TaskMoney[0], resultTaskDataList[i].TaskMoney[1] + 1),
                };
            }
            return taskItemDIct;
        }
    }
}
