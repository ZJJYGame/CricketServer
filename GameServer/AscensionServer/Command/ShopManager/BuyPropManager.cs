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


            if (roleAssets.RoleGold >= (shopDict[roleShopDTO.PropID].PropPrice* roleShopDTO.PropNum))
            {
                Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(roleShopDTO) + "商店数据" + Utility.Json.ToJson(shopDict));
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
                xRCommon.xRS2CSend(roleShopDTO.RoleID, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAssets);

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
            if (gold > 0&& roleAssets.RoleGold >= gold)
            {
                roleAssets.RoleGold -= gold;
                NHibernateQuerier.Update(roleAssets);
                var dataDict = xRCommon.xRS2CParams();
                dataDict.Add((byte)ParameterCode.RoleAsset, roleAssets);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)ShopOperate.Buy, Utility.Json.ToJson(dataDict));
                Utility.Debug.LogInfo("YZQData" + Utility.Json.ToJson(dataDict));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Success, dict);
            }else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Fail,xRCommonTip.xR_err_VerifyAssets);
        }

        public static void GetAwarad(RolepPropDTO roleShopDTO)
        {
            //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ADAward>>(out var adAwardDict);
            switch ((ADAwardType)roleShopDTO.PropType)
            {
                case ADAwardType.gold:
                    GoldAward(roleShopDTO);
                    break;
                case ADAwardType.SkillBook:
                    SkillAward(roleShopDTO);
                    break;
                case ADAwardType.Prop:
                    break;
                case ADAwardType.Cricket:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 领取金币广告奖励
        /// </summary>
        /// <param name="roleShopDTO"></param>
        public static void GoldAward(RolepPropDTO roleShopDTO)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ADAward>>(out var adAwardDict);
            var result = adAwardDict.TryGetValue(roleShopDTO.PropID,out var aDAward);
            if (result)
            {
                Random random = new Random();
                var num = random.Next(aDAward.AddNumber[0], aDAward.AddNumber[0] + 1);
                UpdateRoleAssets(roleShopDTO.RoleID, num);
            }
            else
            {
                xRCommon.xRS2CSend(roleShopDTO.RoleID, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAwardType);
            }
        }
        /// <summary>
        /// 领取技能广告奖励
        /// </summary>
        /// <param name="roleShopDTO"></param>
        public static void SkillAward(RolepPropDTO roleShopDTO)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ADAward>>(out var adAwardDict);
            var result = adAwardDict.TryGetValue(roleShopDTO.PropID, out var aDAward);
            if (result)
            {
                Random random = new Random();
                var num = random.Next(aDAward.AddNumber[0], aDAward.AddNumber[0] + 1);
                InventoryManager.xRAddInventory(roleShopDTO.RoleID,new Dictionary<int, ItemDTO>() { { num, new ItemDTO() {ItemAmount=1 } } });
            }
            else
            {
                xRCommon.xRS2CSend(roleShopDTO.RoleID, (ushort)ATCmd.SyncShop, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAwardType);
            }
        }
        /// <summary>
        /// 领取道具奖励
        /// </summary>
        /// <param name="roleShopDTO"></param>
        public static void PropAward(RolepPropDTO roleShopDTO)
        {
            Random random = new Random();
            var num = random.Next(0,1001);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, ADAward>>(out var adAwardDict);
            if (num<=400)
            {
              num=  random.Next(adAwardDict[1703].AddNumber[0], adAwardDict[1703].AddNumber[1]);
                InventoryManager.xRAddInventory(roleShopDTO.RoleID, new Dictionary<int, ItemDTO>() { { num, new ItemDTO() { ItemAmount = 1 } } });
            }
        }
        /// <summary>
        /// 领取蛐蛐奖励
        /// </summary>
        /// <param name="roleShopDTO"></param>
        public static void CricketAward(RolepPropDTO roleShopDTO)
        {

        }
        public enum ADAwardType
        {
            gold=1,
            SkillBook=2,
            Prop=3,
            Cricket=4
        }
    }
}
