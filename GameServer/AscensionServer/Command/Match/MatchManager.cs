using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AscensionProtocol;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    [CustomeModule]
    public class MatchManager:Module<MatchManager>
    {
        /// <summary>
        ///直接加入大池子
        /// </summary>
        public List<MatchDTO> matchSetDict = new List<MatchDTO>();

        public MatchDTO matchSet;

        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncMatch, C2SMatch);

        private void C2SMatch(OperationData opData)
        {
            Utility.Debug.LogInfo("进行匹配==>" + (opData.DataMessage.ToString()));
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            var roleSet = Utility.Json.ToObject<Dictionary<byte, MatchDTO>>(data.Values.ToList()[0].ToString());
            switch ((SubOperationCode)data.Keys.ToList()[0])
            {
                case SubOperationCode.None:
                    break;
                case SubOperationCode.Get:
                    InitMatch(roleSet[(byte)ParameterCode.RoleMatch]);
                    break;
                case SubOperationCode.Add:
                    break;
                case SubOperationCode.Update:
                    StartMatch(roleSet[(byte)ParameterCode.RoleMatch]);
                    break;
                case SubOperationCode.Remove:
                    break;
                case SubOperationCode.Verify:
                    CanelPlayer(roleSet[(byte)ParameterCode.RoleMatch]);
                    break;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="match"></param>
        public void InitMatch(MatchDTO match)
        {
            MatchDTO matchDto = new MatchDTO();
          
            if (matchSetDict.Count != 0)
            {
                var setData = matchSetDict.Find(xr => xr.selfCricketData.LevelID >= match.selfCricketData.LevelID || xr.selfCricketData.LevelID < match.selfCricketData.LevelID);
                if (setData !=null)
                {
                    matchDto.selfData = match.selfData;
                    matchDto.selfCricketData = match.selfCricketData;
                    matchDto.otherData = setData.selfData;
                    matchDto.otherCricketData = setData.selfCricketData;

                    Utility.Debug.LogInfo("匹配id！！！"+ matchDto.selfData.RoleID+""+ matchDto.otherData.RoleID);
                    var pareams = xRCommon.xRS2CParams();
                    pareams.Add((byte)ParameterCode.RoleMatch, Utility.Json.ToJson(matchDto));
                    var subOp = xRCommon.xRS2CSub();
                    subOp.Add((byte)SubOperationCode.Get, pareams);
                    for (int ov = 0; ov < 2; ov++)
                    {
                        if (ov==0)
                            xRCommon.xRS2CSend(matchDto.selfData.RoleID, (byte)ATCmd.SyncMatch, (byte)ReturnCode.Success, subOp);
                        if (ov ==1)
                            xRCommon.xRS2CSend(matchDto.otherData.RoleID, (byte)ATCmd.SyncMatch, (byte)ReturnCode.Success, subOp);
                    }
                    matchSetDict.Remove(setData);
                    matchSetDict.Remove(match);

                    //TODO
                    GameManager.CustomeModule<BattleRoomManager>().CreateRoom(matchDto);
                    TimerManager matchManager = new TimerManager(1500);
                    matchManager.BattleStartTimer();
                }
                else
                {
                    Utility.Debug.LogInfo("没有匹配成功！！！");
                }
            }
            else
            {
                matchDto.RoleId = match.selfData.RoleID;
                matchDto.selfData = match.selfData;
                matchDto.selfCricketData = match.selfCricketData;
                matchDto.otherData = match.otherData;
                matchDto.otherCricketData = match.otherCricketData;
                var setDataRes = matchSetDict.Find(xr => xr.RoleId == match.selfData.RoleID);
                if (setDataRes != null)
                    matchSetDict.Remove(setDataRes);
                matchSetDict.Add(matchDto);
            }
        }

        /// <summary>
        /// 开始匹配人机
        /// </summary>
        public void StartMatch(MatchDTO match)
        {
            var xrRemove = matchSetDict.Find(x => x.RoleId == match.RoleId);
            if (xrRemove != null)
                matchSetDict.Remove(xrRemove);
            MatchDTO matchDto = new MatchDTO();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MachineData>>(out var machineData);
            if (machineData.ContainsKey(match.selfCricketData.RankID))
            {
                var setData = machineData[match.selfCricketData.RankID];
                matchDto.selfData = match.selfData;
                matchDto.selfCricketData = match.selfCricketData;
                matchDto.otherData = new RoleDTO() { RoleName = setData.UserName };
                matchDto.otherCricketData = new CricketDTO() { CricketName = setData.CricketName, RankID = setData.RankID };
                var pareams = xRCommon.xRS2CParams();
                pareams.Add((byte)ParameterCode.RoleMatch, Utility.Json.ToJson(matchDto));
                var subOp = xRCommon.xRS2CSub();
                subOp.Add((byte)SubOperationCode.Get, pareams);
                xRCommon.xRS2CSend(matchDto.selfData.RoleID, (byte)ATCmd.SyncMatch, (byte)ReturnCode.Success, subOp);

                //TODO
                GameManager.CustomeModule<BattleRoomManager>().CreateRoom(matchDto, setData);
                TimerManager matchManager = new TimerManager(1500);
                matchManager.BattleStartTimer();
            }
        }

        /// <summary>
        /// 取消匹配
        /// </summary>
        /// <param name="match"></param>
        public void CanelPlayer(MatchDTO match)
        {
            var xrRemove = matchSetDict.Find(x => x.RoleId == match.RoleId);
            if (xrRemove != null)
                matchSetDict.Remove(xrRemove);
        }
       

        /// <summary>
        /// 战斗结算
        /// </summary>
        public void BattleCombat()
        {

        }


    }
}
