using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using Protocol;
namespace AscensionServer
{
    [CustomeModule]
    public class ShopManager : Module<ShopManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncCricket, null);
        }

        public void C2SShop(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            Utility.Debug.LogInfo("yzqData请求蛐蛐属性:" + Utility.Json.ToJson(data));
            foreach (var item in data)
            {
                var dict = Utility.Json.ToObject<Dictionary<byte, object>>(item.Value.ToString());
                switch ((ShopOperate)item.Key)
                {
                    case ShopOperate.Buy:

                        break;
                    default:
                        break;
                }
            }
        }
    }
}
