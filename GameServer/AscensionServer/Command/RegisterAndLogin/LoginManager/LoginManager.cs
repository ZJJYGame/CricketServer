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
        }

        public void C2SLogin(OperationData opData)
        {
            var message =Utility.Json.ToObject<User>(opData.DataMessage.ToString());
            var dp = opData.DataContract as DataParameters;
            dp.Messages.TryGetValue((byte)ParameterCode.ClientPeer, out var peer);

            Utility.Debug.LogInfo("yzqData登录账号：" + message.Account+"密码：" + message.Password);
            LoginHandler.LoginRole(message.Account, message.Password,peer );
        }

        public void S2CLogin(int roleid, string message, ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (byte)returnCode;
            operationData.OperationCode = (ushort)ATCmd.Login;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, operationData);
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
