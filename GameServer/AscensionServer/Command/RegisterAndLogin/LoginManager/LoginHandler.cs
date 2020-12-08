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
   public static  class LoginHandler
    {
        public static void LoginRole(string account, string password,object peer)
        {
            NHCriteria nHCriteriauser = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", account);
            var user = NHibernateQuerier.CriteriaSelect<User>(nHCriteriauser);
            if (user != null)
            {
                if (user.Password == password)
                {
                    NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", user.RoleID);
                    var role = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRole);
                    var roleAsset = NHibernateQuerier.CriteriaSelect<RoleAssets>(nHCriteriaRole);
                    //var roleEntity= RoleEntity.Create(role.RoleID, (peer as IPeerEntity).SessionId, role);
                    //GameManager.CustomeModule<RoleManager>().TryAdd(role.RoleID, roleEntity);
                    //GameManager.CustomeModule<LoginManager>().S2CLogin((peer as IPeerEntity).SessionId, Utility.Json.ToJson(role), ReturnCode.Success);
                    #region 
                    var roleEntity = RoleEntity.Create(role.RoleID, (peer as IPeerEntity).SessionId, role);
                    IPeerEntity peerAgent;
                    var result = GameManager.CustomeModule<PeerManager>().TryGetValue((peer as IPeerEntity).SessionId, out peerAgent);
                    if (result)
                    {
                        var remoteRoleType = typeof(RoleEntity);
                        var exist = peerAgent.ContainsKey(remoteRoleType);
                        if (!exist)
                        {
                            GameManager.CustomeModule<RoleManager>().TryAdd(role.RoleID, roleEntity);
                            peerAgent.TryAdd(remoteRoleType, roleEntity);
                            (peer as AscensionPeer).RoleEntity = roleEntity;
                            Utility.Debug.LogInfo("yzqData登录成功2");
                            GameManager.CustomeModule<LoginManager>().S2CLogin(role.RoleID, Utility.Json.ToJson(role), ReturnCode.Success);
                        }
                        else
                        {
                            Utility.Debug.LogInfo("yzqData登录成功3");
                            //TODO提示账号已在线阻止登陆
                            GameManager.CustomeModule<LoginManager>().S2CLogin(role.RoleID, "账号已登录", ReturnCode.Fail);
                        }
                    }
                    #endregion
                }
            }
        }
    }
}
