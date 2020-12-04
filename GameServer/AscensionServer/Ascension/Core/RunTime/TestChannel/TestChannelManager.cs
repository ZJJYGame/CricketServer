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
    public class TestChannelManager : Module<TestChannelManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(Protocol.ProtocolDefine.OPR_TESTCAHNNEL, PrintTestMsg);
        }
        void PrintTestMsg(Protocol.OperationData opData)
        {
            Utility.Debug.LogWarning(opData.DataMessage);
            OperationData opDataTest = new OperationData();
            var dp = opData.DataContract as DataParameters;
            dp.Messages.TryGetValue((byte)ParameterCode.ClientPeer, out var peer);
            Utility.Debug.LogWarning($"SessionId:{(peer as IPeerEntity).SessionId}");

            opDataTest.DataMessage = "服务器 倒计时10 秒  over！ ";
            opDataTest.OperationCode = ProtocolDefine.OPR_TESTCAHNNEL;
            GameManager.CustomeModule<PeerManager>().SendMessage((peer as IPeerEntity).SessionId, opDataTest);
        }
    }
}
