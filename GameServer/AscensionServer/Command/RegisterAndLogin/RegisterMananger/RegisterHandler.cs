using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
using Cosmos.Reference;
namespace AscensionServer
{
    public static  class RegisterHandler
    {
        public static void RegisterRole(string account,string password)
        {
            NHCriteria nHCriteriaAccount = GameManager.CustomeModule<ReferencePoolManager>().Spawn<NHCriteria>().SetValue("Account", account);
            bool isExist = NHibernateQuerier.Verify<User>(nHCriteriaAccount);
            var userObj = GameManager.ReferencePoolManager.Spawn<User>();
            var role = GameManager.ReferencePoolManager.Spawn<Role>();
            if (!isExist)
            {
                userObj = NHibernateQuerier.Insert(userObj);
                NHCriteria nHCriteriaUUID = GameManager.CustomeModule<ReferencePoolManager>().Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);

                role = NHibernateQuerier.Insert<Role>(role);
                userObj.RoleID = role.RoleID;
                NHibernateQuerier.Update(userObj);
                GameManager.CustomeModule<RegisterMananger>().S2CRegister(role.RoleID, Utility.Json.ToJson(role), AscensionProtocol.ReturnCode.Success);
            }
            else
            {
                GameManager.CustomeModule<RegisterMananger>().S2CRegister(role.RoleID, "账号已存在",AscensionProtocol.ReturnCode.Fail);
            }
        }
    }
}
