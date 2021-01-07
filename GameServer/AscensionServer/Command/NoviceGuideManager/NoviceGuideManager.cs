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
    public class NoviceGuideManager : Module<NoviceGuideManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncGuide, C2SNoviceGuide);
        }
        public void C2SNoviceGuide(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            foreach (var item in data)
            {
                var roleData = Utility.Json.ToObject<Dictionary<byte, object>>(item.Value.ToString());
                switch ((GuideOperateType)item.Key)
                {
                    case GuideOperateType.Update:
                        var role= Utility.Json.ToObject<Role>(roleData[(byte)ParameterCode.Role].ToString());
                        if (role!=null)
                        {
                            if (role.RoleID!=0)
                            {
                                NHibernateQuerier.Update(role);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
