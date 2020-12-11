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

            if (roleAssets.RoleID >= (shopDict[roleShopDTO.PropID].PropPrice* roleShopDTO.PropNum))
            {
                roleAssets.RoleID -= (shopDict[roleShopDTO.PropID].PropPrice * roleShopDTO.PropNum);
                Dictionary<int, ItemDTO> itemDict = new Dictionary<int, ItemDTO>();
                ItemDTO itemDTO = new ItemDTO();
                itemDTO.ItemAmount = roleShopDTO.PropNum;
                itemDict.Add(roleShopDTO.PropID, itemDTO);
                InventoryManager.xRAddInventory(roleShopDTO.RoleID, itemDict);
            }

        }
    }
}
