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
    public partial class InventoryManager
    {

        public static void xRGetInventory(int roleId)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteriaSelectMethod<Inventory>(nHcriteria);
                Utility.Debug.LogInfo("老陆==>" + xRserver.ItemDict);
                xRCommon.xRS2CParams().Add((byte)ParameterCode.RoleInventory, xRserver.ItemDict);
                xRCommon.xRS2CSub().Add((byte)subInventoryOp.Get, xRCommon.xRS2CParams());
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncInventory,(byte)ReturnCode.Success);
            }
        }

    }
}
