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
   public partial  class ExplorationManager:Module<ExplorationManager>
    {
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncExploration, C2SExploration);

        private void C2SExploration(OperationData opData)
        {
            Utility.Debug.LogInfo("老陆探索==>" + (opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, ExplorationDTO>>(data.Values.ToList()[0].ToString());
            switch ((SubOperationCode)data.Keys.ToList()[0])
            {
                case SubOperationCode.None:
                    break;
                case SubOperationCode.Get:
                    ExplorationManager.xRGetExploration(roleSet[(byte)ParameterCode.RoleExploration].RoleID);
                    break;
                case SubOperationCode.Add:
                    ExplorationManager.xRAddExploration(roleSet[(byte)ParameterCode.RoleExploration].RoleID, roleSet[(byte)ParameterCode.RoleExploration].ExplorationItemDict, roleSet[(byte)ParameterCode.RoleExploration].CatchPropAndTimeProp);
                    break;
                case SubOperationCode.Update:
                    ExplorationManager.xRUpdateExploration(roleSet[(byte)ParameterCode.RoleExploration].RoleID, roleSet[(byte)ParameterCode.RoleExploration].ExplorationItemDict, roleSet[(byte)ParameterCode.RoleExploration].CatchPropAndTimeProp);
                    break;
                case SubOperationCode.Remove:
                    ExplorationManager.xRRemoveExploration(roleSet[(byte)ParameterCode.RoleExploration].RoleID, roleSet[(byte)ParameterCode.RoleExploration].ExplorationItemDict);
                    break;
                case SubOperationCode.Verify:
                    ExplorationManager.xRVerifyExploration(roleSet[(byte)ParameterCode.RoleExploration].RoleID, roleSet[(byte)ParameterCode.RoleExploration].UnLockDict);
                    break;
            }

        }
    }
}
