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
        /// <summary>
        /// 获取背包
        /// </summary>
        /// <param name="roleId"></param>
        public static void xRGetInventory(int roleId)
        {
            var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            if (xRCommon.xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCommon.xRCriteriaSelectMethod<Inventory>(nHcriteria);
                Utility.Debug.LogInfo("老陆==>" + xRserver.ItemDict);
                var tt = xRCommon.xRS2CParams();
                tt.Add((byte)ParameterCode.RoleInventory, xRserver.ItemDict);
                var dd = xRCommon.xRS2CSub();
                dd.Add((byte)subInventoryOp.Get, tt);
                xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncInventory,(byte)ReturnCode.Success, dd);
            }
        }



    }
}
