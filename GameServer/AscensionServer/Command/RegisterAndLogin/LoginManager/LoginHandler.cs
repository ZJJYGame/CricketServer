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
        public static void LoginRole(string account, string password)
        {
            NHCriteria nHCriteriaRole = GameManager.CustomeModule<ReferencePoolManager>().Spawn<NHCriteria>().SetValue("Account", account);
            var user = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaRole);
            if (user != null)
            {
                if (user.Password == password)
                {

                }
            }
            else
            {
            }
        }
    }
}
