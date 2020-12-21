using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Protocol;
using Cosmos;
namespace AscensionServer
{
   public static class BuyPropManager
    {
        public static void BuyProp(RolepPropDTO roleShopDTO)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", roleShopDTO.RoleID);
            var roleAssets = xRCommon.xRCriteria<RoleAssets>(nHCriteria);
            Utility.Debug.LogInfo("YZQ數據庫映射"+Utility.Json.ToJson(roleAssets));
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, Shop>>(out var shopDict);

            Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(roleShopDTO)+"商店数据"+Utility.Json.ToJson(shopDict));
            if (roleAssets.RoleGold >= (shopDict[roleShopDTO.PropID].PropPrice* roleShopDTO.PropNum))
            {
                roleAssets.RoleGold -= (shopDict[roleShopDTO.PropID].PropPrice * roleShopDTO.PropNum);
                Dictionary<int, ItemDTO> itemDict = new Dictionary<int, ItemDTO>();
                ItemDTO itemDTO = new ItemDTO();
                itemDTO.ItemAmount = roleShopDTO.PropNum;
                itemDict.Add(roleShopDTO.PropID, itemDTO);
                InventoryManager.xRAddInventory(roleShopDTO.RoleID, itemDict);
                NHibernateQuerier.Update(roleAssets);


                var dataDict= xRCommon.xRS2CParams();
                dataDict.Add((byte)ParameterCode.RoleAsset, roleAssets);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)ShopOperate.Buy, Utility.Json.ToJson(dataDict));
                Utility.Debug.LogInfo("YZQData"+Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleShopDTO.RoleID,(ushort)ATCmd.SyncShop,(byte)ReturnCode.Success, dict);
            }else
                xRCommon.xRS2CSend(roleShopDTO.RoleID, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);

        }

        /// <summary>
        /// 玩家获得金币
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="gold"></param>
        public static void UpdateRoleAssets(int roleid,int gold)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleAssets = xRCommon.xRCriteria<RoleAssets>(nHCriteria);
            if (gold>0)
            {
                roleAssets.RoleGold += gold;
                NHibernateQuerier.Update(roleAssets);
                var dataDict = xRCommon.xRS2CParams();
                dataDict.Add((byte)ParameterCode.RoleAsset, roleAssets);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)ShopOperate.Buy, Utility.Json.ToJson(dataDict));
                Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Success, dict);
            }
        }
        /// <summary>
        /// 玩家消耗金币
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="gold"></param>
        public static void ExpenseRoleAssets(int roleid, int gold)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", roleid);
            var roleAssets = xRCommon.xRCriteria<RoleAssets>(nHCriteria);
            if (gold > 0)
            {
                roleAssets.RoleGold -= gold;
                NHibernateQuerier.Update(roleAssets);
                var dataDict = xRCommon.xRS2CParams();
                dataDict.Add((byte)ParameterCode.RoleAsset, roleAssets);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)ShopOperate.Buy, Utility.Json.ToJson(dataDict));
                Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Success, dict);
            }
        }
    }
}
