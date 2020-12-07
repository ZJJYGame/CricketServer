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

            Utility.Debug.LogInfo("yzqData"+message.Account+"////"+ message.Password);
          LoginHandler.LoginRole(message.Account, message.Password);
        }

        public void S2CLoginr(int roleid, string message, ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (byte)returnCode;
            operationData.OperationCode = (ushort)ATCmd.Login;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, operationData);
        }
    }
}
