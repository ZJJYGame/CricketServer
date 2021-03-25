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
        Dictionary<int, RankDTO> rankDict = new Dictionary<int, RankDTO>();
        Dictionary<int, BattleCombatDTO> winsDict = new Dictionary<int, BattleCombatDTO>();
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
            //rankDict.Clear();
            if (rankDict.Count == 0)
            {
                //var nHcriteria = xRCommon.xRNHCriteria("RoleID", roleId);
                //var xRserver = xRCommon.xRCriteria<Role>(nHcriteria);
                var tableRole = NHibernateQuerier.GetTable<Role>();
                Dictionary<int, Role> roleDict = tableRole.ToDictionary(key => key.RoleID, value => value);
                var tableCricket = NHibernateQuerier.GetTable<Cricket>();
                tableCricket = tableCricket.OrderByDescending(o => o.RankID).ToList();//降序
                foreach (var info in tableCricket)//tableCricket.Count > 100 ? 100 : tableCricket.Count
                {
                    if (rankDict.Count >= 100)
                        break;
                    rankDict[info.ID] = new RankDTO { RoleID = info.Roleid,RoleHeadIcon= roleDict[info.Roleid].HeadPortrait,RoleName=roleDict[info.Roleid].RoleName,CricketHeadIcon=info.HeadPortraitID, CricketName = info.CricketName, Duanwei = info.RankID };
                }
            }

            if (winsDict.Count == 0)
            {
                var tableBattleCombat= NHibernateQuerier.GetTable<BattleCombat>();
                tableBattleCombat = tableBattleCombat.OrderByDescending(o => o.MatchWon).ToList();
                foreach (var item in tableBattleCombat)
                {
                    if (winsDict.Count >= 100)
                        break;
                    winsDict[item.RoleID] = new BattleCombatDTO { RoleID = item.RoleID, MatchWon = item.MatchWon, RoleName = item.RoleName };
                }
            }

            var pareams = xRCommon.xRS2CParams();
            pareams.Add((byte)ParameterCode.RoleRank, Utility.Json.ToJson(rankDict));
            pareams.Add((byte)ParameterCode.WinRank, Utility.Json.ToJson(winsDict));
            var subOp = xRCommon.xRS2CSub();
            subOp.Add((byte)SubOperationCode.Get, pareams);
            xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncRank, (byte)ReturnCode.Success, subOp);
        }

        public void ClearRankDict()
        {
            rankDict.Clear();
            winsDict.Clear();
        }
    }
}
