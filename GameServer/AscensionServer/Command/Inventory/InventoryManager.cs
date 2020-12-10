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
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncInventory, C2SInventory);
        }

        private void C2SInventory(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, string>>(opData.DataMessage.ToString());
            foreach (var subOp in data)
            {
                switch ((subInventoryOp)subOp.Key)
                {
                    case subInventoryOp.None:
                        break;
                    case subInventoryOp.Get:
                        break;
                    case subInventoryOp.Add:
                        break;
                    case subInventoryOp.Update:
                        break;
                    case subInventoryOp.Remove:
                        break;
                    case subInventoryOp.Verify:
                        break;
                }
            }

        }
    }
}
