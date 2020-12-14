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
    public partial  class InventoryManager:Module<InventoryManager>
    {
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncInventory, C2SInventory);

        private void C2SInventory(OperationData opData)
        {
            Utility.Debug.LogInfo("老陆==>" +(opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, InventoryDTO>>(data.Values.ToList()[0].ToString());
            switch ((subInventoryOp)data.Keys.ToList()[0])
            {
                case subInventoryOp.None:
                    break;
                case subInventoryOp.Get:
                    InventoryManager.xRGetInventory(roleSet[(byte)ParameterCode.RoleInventory].RoleID);
                    break;
                case subInventoryOp.Add:
                    InventoryManager.xRAddInventory(roleSet[(byte)ParameterCode.RoleInventory].RoleID, roleSet[(byte)ParameterCode.RoleInventory].ItemDTO);
                    break;
                case subInventoryOp.Update:
                    InventoryManager.xRUpdateInventory(roleSet[(byte)ParameterCode.RoleInventory].RoleID, roleSet[(byte)ParameterCode.RoleInventory].ItemDTO);
                    break;
                case subInventoryOp.Remove:
                    break;
                case subInventoryOp.Verify:
                    break;  
            }
        }
    }
}
