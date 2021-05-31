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
    public partial class RankManager : Module<RankManager>
    {
        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncRank, C2SRank);
        //存储段位信息
        LinkedList<RankDTO> rankLinked = new LinkedList<RankDTO>();
        //存储胜场信息
        LinkedList<BattleCombatDTO> winLinked = new LinkedList<BattleCombatDTO>();
        ///第一个int为rankid，第二个为circketid(即ID)
        SortedDictionary<int, SortedDictionary<int, Cricket>> sortedCricket = new SortedDictionary<int, SortedDictionary<int, Cricket>>();
        //第一个int为总胜场，第二个为cricketid
        SortedDictionary<int, SortedDictionary<int, BattleCombat>> winCricket = new SortedDictionary<int, SortedDictionary<int, BattleCombat>>();
        //临时把所有的蛐蛐数据存入到一个临时list中
        List<Cricket> tempRankList = new List<Cricket>();
        //临时把所有胜场信息存到一个临时list中
        List<BattleCombat> tempBattle = new List<BattleCombat>();

        bool isOne = true;
        bool isTwo = true;
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
            var tableRole = NHibernateQuerier.GetTable<Role>();
            Dictionary<int, Role> roleDict = tableRole.ToDictionary(key => key.RoleID, value => value);
            var tableCricket = NHibernateQuerier.GetTable<Cricket>();
            var tableCricketData = tableCricket.ToList();


            if (tableCricket.Count > 0)
            {
                //先储存并排序好所有的数据
                for (int i = 0; i < tableCricket.Count; i++)
                {
                    if (!sortedCricket.TryGetValue(tableCricketData[i].RankID, out var sortedDic))
                    {
                        //  isOne = true;
                        sortedDic = new SortedDictionary<int, Cricket> { { tableCricketData[i].ID, tableCricketData[i] } };
                        sortedCricket.Add(tableCricketData[i].RankID, sortedDic);
                    }
                    else
                    {
                        if (!sortedCricket[tableCricketData[i].RankID].TryGetValue(tableCricketData[i].ID, out var tableCriDic))
                        {
                            //  isOne = true;
                            tableCriDic = tableCricketData[i];
                            sortedCricket[tableCricketData[i].RankID].Add(tableCricketData[i].ID, tableCriDic);
                        }
                    }
                }

                if (isOne)
                {
                    for (int i = 0; i < sortedCricket.Count; i++)
                    {
                        for (int j = 0; j < sortedCricket.ElementAt(i).Value.Count; j++)
                        {
                            var cricket = sortedCricket.ElementAt(i).Value.ElementAt(j).Value;
                            //把这些数据存到一个临时的数组中
                            //这一步是避免出现一个相同的rankid对应的最大数据为100而导致整个json不止100条的问题（emmm可能说的有点饶

                            tempRankList.Add(cricket);
                        }
                    }

                    //这一步是把所有的数据按照段位id从大到小排（默认是从小到大
                    tempRankList = tempRankList.OrderByDescending(t => t.RankID).ToList();
                    for (int i = 0; i < tempRankList.Count; i++)
                    {
                        if (i < 100)
                        {
                            var cricket = tempRankList[i];
                            //到最后再把数据存到linkedlist中
                            rankLinked.AddLast(new RankDTO { RoleID = cricket.Roleid, RoleHeadIcon = roleDict[cricket.Roleid].HeadPortrait, RoleName = roleDict[cricket.Roleid].RoleName, CricketHeadIcon = cricket.HeadPortraitID, CricketName = cricket.CricketName, Duanwei = cricket.RankID });
                        }
                    }
                    isOne = false;
                }
            }

            //胜场榜和段位榜写法一样的
            var tableBattleCombat = NHibernateQuerier.GetTable<BattleCombat>();
            var tableBattleCombatData = tableBattleCombat.ToList();

            if (tableBattleCombat.Count > 0)
            {
                for (int i = 0; i < tableBattleCombat.Count; i++)
                {
                    if (!winCricket.TryGetValue(tableBattleCombatData[i].MatchWon, out var cricketSort))
                    {
                        //  isTwo = true;
                        cricketSort = new SortedDictionary<int, BattleCombat> { { tableBattleCombatData[i].RoleID, tableBattleCombatData[i] } };
                        winCricket.Add(tableBattleCombatData[i].MatchWon, cricketSort);
                    }
                    else
                    {
                        if (!winCricket[tableBattleCombatData[i].MatchWon].TryGetValue(tableBattleCombatData[i].RoleID, out var tableWon))
                        {
                            // isTwo = true;
                            tableWon = tableBattleCombatData[i];
                            winCricket[tableBattleCombatData[i].MatchWon].Add(tableBattleCombatData[i].RoleID, tableWon);
                        }
                    }
                }

                if (isTwo)
                {
                    for (int i = 0; i < winCricket.Count; i++)
                    {
                        for (int j = 0; j < winCricket.ElementAt(i).Value.Count; j++)
                        {
                            var won = winCricket.ElementAt(i).Value.ElementAt(j).Value;
                            tempBattle.Add(won);
                        }
                    }
                    tempBattle = tempBattle.OrderByDescending(t => t.MatchWon).ToList();
                    for (int i = 0; i < tempBattle.Count; i++)
                    {
                        if (i < 100)
                        {
                            var won = tempBattle[i];
                            winLinked.AddLast(new BattleCombatDTO { RoleID = won.RoleID, MatchWon = won.MatchWon, RoleName = roleDict[won.RoleID].RoleName, RoleHeadIcon = roleDict[won.RoleID].HeadPortrait });
                        }
                    }
                    isTwo = false;
                }
            }

            Utility.Debug.LogInfo(Utility.Json.ToJson(rankLinked));
            var pareams = xRCommon.xRS2CParams();
            pareams.Add((byte)ParameterCode.RoleRank, Utility.Json.ToJson(rankLinked));
            pareams.Add((byte)ParameterCode.WinRank, Utility.Json.ToJson(winLinked));
            var subOp = xRCommon.xRS2CSub();
            subOp.Add((byte)SubOperationCode.Get, pareams);
            xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncRank, (byte)ReturnCode.Success, subOp);
        }

        public void ClearRankDict()
        {
            isOne = true;
            isTwo = true;
            rankLinked.Clear();
            winLinked.Clear();
        }
    }
}
