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
   public class LoginManager : Module<LoginManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.Login, C2SLogin);
            CommandEventCore.instance.AddEventListener((ushort)ATCmd.Logoff, C2SLoginOff);
        }

        public void C2SLogin(OperationData opData)
        {
            var message =Utility.Json.ToObject<User>(opData.DataMessage.ToString());
            var dp = opData.DataContract;
            dp.Messages.TryGetValue((byte)ParameterCode.ClientPeer, out var peer);

            Utility.Debug.LogInfo("yzqData登录账号：" + message.Account+"密码：" + message.Password);
            LoginHandler.LoginRole(message.Account, message.Password,peer );
        }
        public void C2SLoginOff(OperationData opData)
        {
            var role = Utility.Json.ToObject<Role>(opData.DataMessage.ToString());
            var dp = opData.DataContract;
            dp.Messages.TryGetValue((byte)ParameterCode.ClientPeer, out var peer);
            LoginHandler.LoginOffRole(role,peer);
        }

        public void S2CLoginOff(int seesionid,string message,ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (short)returnCode;
            operationData.OperationCode = (ushort)ATCmd.Logoff;
            Utility.Debug.LogError("向" + seesionid + "发送下线通知");
            GameManager.CustomeModule<PeerManager>().SendMessage(seesionid, operationData);
        }

        public void S2CLogin(int sessionId, string message, ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (byte)returnCode;
            operationData.OperationCode = (ushort)ATCmd.Login;
            GameManager.CustomeModule<PeerManager>().SendMessage(sessionId, operationData);
        }
        public void S2CLogin(int seesionid)
        {
            Utility.Debug.LogInfo("yzqData账号错误请重试");
            OperationData operationData = new OperationData();
            operationData.DataMessage = "账号错误请重试";
            operationData.ReturnCode = (byte)ReturnCode.Fail;
            operationData.OperationCode = (ushort)ATCmd.Login;
            GameManager.CustomeModule<PeerManager>().SendMessage(seesionid, operationData);
        }
    }
}
