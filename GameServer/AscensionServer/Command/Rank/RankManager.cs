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
        SortedDictionary<int, RankDTO> rankDict = new SortedDictionary<int, RankDTO>();
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
                SortedList<int, Cricket> cricketSortedList = new SortedList<int, Cricket>();
                foreach(var item in tableCricket)
                {
                    cricketSortedList.Add(item.RankID, item);
                }
                int indexCount = cricketSortedList.Count < 100 ? cricketSortedList.Count : 100;
                for (int i = indexCount-1; i >=0; i--)
                {
                    rankDict[cricketSortedList[i].ID] = new RankDTO { RoleID = cricketSortedList[i].Roleid, RoleHeadIcon = roleDict[cricketSortedList[i].Roleid].HeadPortrait, RoleName = roleDict[cricketSortedList[i].Roleid].RoleName, CricketHeadIcon = cricketSortedList[i].HeadPortraitID, CricketName = cricketSortedList[i].CricketName, Duanwei = cricketSortedList[i].RankID };
                }

                var tableBattleCombat = NHibernateQuerier.GetTable<BattleCombat>();
                SortedList<int, BattleCombat> battleCombatSortedList = new SortedList<int, BattleCombat>();
                foreach(var item in tableBattleCombat)
                {
                    battleCombatSortedList.Add(item.MatchWon, item);
                }
                indexCount = battleCombatSortedList.Count < 100 ? battleCombatSortedList.Count : 100;
                for (int i = indexCount-1; i >= 0; i--)
                {
                    winsDict[battleCombatSortedList[i].RoleID] = new BattleCombatDTO { RoleID = battleCombatSortedList[i].RoleID, MatchWon = battleCombatSortedList[i].MatchWon, RoleName = roleDict[battleCombatSortedList[i].RoleID].RoleName, RoleHeadIcon = roleDict[battleCombatSortedList[i].RoleID].HeadPortrait };
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
