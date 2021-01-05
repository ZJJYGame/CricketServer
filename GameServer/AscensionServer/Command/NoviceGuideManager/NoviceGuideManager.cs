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
    public class NoviceGuideManager : Module<NoviceGuideManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncCricket, C2SNoviceGuide);
        }
        public void C2SNoviceGuide(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            foreach (var item in data)
            {
                switch ((GuideOperateType)item.Key)
                {
                    case GuideOperateType.Update:

                        break;
                    default:
                        break;
                }
            }
        }
    }
}
