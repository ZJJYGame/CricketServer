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
        public static void BuyProp(RoleShopDTO roleShopDTO)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", roleShopDTO.RoleID);
            var roleAssets = xRCommon.xRCriteria<RoleAssets>(nHCriteria);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, Shop>>(out var shopDict);

            Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(roleShopDTO));
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
    }
}
