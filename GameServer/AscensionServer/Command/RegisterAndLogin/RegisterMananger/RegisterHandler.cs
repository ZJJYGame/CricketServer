using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol;
using Cosmos.Reference;
namespace AscensionServer
{
    public static  class RegisterHandler
    {
        public static void RegisterRole(string account,string password,object peer)
        {
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", account);
            Utility.Debug.LogInfo("yzqData发送失败" + nHCriteriaAccount.Value.ToString());
            bool isExist = NHibernateQuerier.Verify<User>(nHCriteriaAccount);
            var userObj = new User() {Account= account,Password= password };
            var role = new Role() { };
            var roleAsset = new RoleAssets();
            if (!isExist)
            {
                userObj = NHibernateQuerier.Insert(userObj);
                NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);

                role = NHibernateQuerier.Insert(role);
                userObj.RoleID = role.RoleID;
                NHibernateQuerier.Update(userObj);
                roleAsset.RoleID = role.RoleID;
                NHibernateQuerier.Insert(roleAsset);
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(role);
                operationData.ReturnCode = (byte)ReturnCode.Success;
                operationData.OperationCode = (ushort)ATCmd.Register;
                Utility.Debug.LogInfo("yzqData发送成功");
                GameManager.CustomeModule<PeerManager>().SendMessage((peer as IPeerEntity).SessionId, operationData);
            }
            else
            {
                OperationData operationData = new OperationData();
                operationData.DataMessage = "账号已存在";
                operationData.ReturnCode = (byte)ReturnCode.Fail;
                operationData.OperationCode = (ushort)ATCmd.Register;
                Utility.Debug.LogInfo("yzqData发送失败");
                GameManager.CustomeModule<PeerManager>().SendMessage((peer as IPeerEntity).SessionId, operationData);
            }
        }
    }
}
