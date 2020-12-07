using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    [CustomeModule]
    public class RegisterMananger : Module<RegisterMananger>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.Register, C2SRegister);
        }

        public void C2SRegister(OperationData opData)
        {
            var message = opData.DataMessage as User;
            var dp = opData.DataContract as DataParameters;
            dp.Messages.TryGetValue((byte)ParameterCode.ClientPeer, out var peer);
            RegisterHandler.RegisterRole(message.Account, message.Password);
        }

        public void S2CRegister(int roleid, string message,ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (byte)returnCode;
            operationData.OperationCode = (ushort)ATCmd.Register;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, operationData);
        }
    }
}
