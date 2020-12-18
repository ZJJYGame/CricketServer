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
   public  partial class TaskManager:Module<TaskManager>
    {
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncTask, C2STask);
        private void C2STask(OperationData opData)
        {
            Utility.Debug.LogInfo("老陆==>" + (opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, TaskDTO>>(data.Values.ToList()[0].ToString());
            switch ((subTaskOp)data.Keys.ToList()[0])
            {
                case subTaskOp.Get:
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
    }
}
