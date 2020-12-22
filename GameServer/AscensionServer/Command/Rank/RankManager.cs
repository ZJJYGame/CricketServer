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
    public partial class RankManager:Module<RankManager>
    {
        public Dictionary<int, RankDTO> rankDict = new Dictionary<int, RankDTO>();
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncRank, C2SRank);

        private void C2SRank(OperationData opData)
        {
            Utility.Debug.LogInfo("老陆排行榜==>" + (opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, RankDTO>>(data.Values.ToList()[0].ToString());
            switch ((SubOperationCode)data.Keys.ToList()[0])
            {
                case SubOperationCode.None:
                    break;
                case SubOperationCode.Get:
                    xrGetRank(roleSet[(byte)ParameterCode.RoleRank].RoleID);
                    break;
                case SubOperationCode.Add:
                    break;
                case SubOperationCode.Update:
                    break;
                case SubOperationCode.Remove:
                    break;
                case SubOperationCode.Verify:
                    break;
            }
        }

        public void xrGetRank(int roleId)
        {
            //var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
            //var xRserver = xRCommon.xRCriteria<Role>(nHcriteria);
            var d = NHibernateQuerier.GetTable<Role>();
            foreach (var item in d)
            {
                Utility.Debug.LogInfo("个人信息" + item.RoleName + item.RoleID);
            }
        }
    }
}
