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
                    var roleCricket = NHibernateQuerier.CriteriaSelect<RoleCricket>(nHCriteriaRole);
                    RoleCricketDTO roleCricketDTO = new RoleCricketDTO();
                    roleCricketDTO.RoleID = roleCricket.RoleID;
                    roleCricketDTO.CricketList =Utility.Json.ToObject<Dictionary<int,int>>(roleCricket.CricketList);
                    roleCricketDTO.TemporaryCrickets = Utility.Json.ToObject<Dictionary<int, int>>(roleCricket.TemporaryCrickets);

                    Dictionary<byte, string> dataDict = new Dictionary<byte, string>();
                    dataDict.Add((byte)ParameterCode.Role,Utility.Json.ToJson(role));
                    dataDict.Add((byte)ParameterCode.RoleAsset, Utility.Json.ToJson(roleAsset));
                    dataDict.Add((byte)ParameterCode.RoleCricket, Utility.Json.ToJson(roleCricketDTO));
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
                            GameManager.CustomeModule<RoleManager>().TryRemove(role.RoleID);
                            var isture=  GameManager.CustomeModule<RoleManager>().TryAdd(role.RoleID, roleEntity);
                            peerAgent.TryAdd(remoteRoleType, roleEntity);
                            Utility.Debug.LogInfo("yzqData登录成功RoleID:"+ role.RoleID+ isture);
                            GameManager.CustomeModule<LoginManager>().S2CLogin(role.RoleID, Utility.Json.ToJson(dataDict), ReturnCode.Success);
                        }
                        else
                        {
                            //TODO提示账号已在线阻止登陆
                            GameManager.CustomeModule<LoginManager>().S2CLogin(role.RoleID, "账号已登录", ReturnCode.Fail);
                        }
                    }
                    #endregion
                }
                else
                {
                    GameManager.CustomeModule<LoginManager>().S2CLogin((peer as IPeerEntity).SessionId);
                }
            }
            else
            {
                GameManager.CustomeModule<LoginManager>().S2CLogin((peer as IPeerEntity).SessionId);
            }
        }
    }
}
