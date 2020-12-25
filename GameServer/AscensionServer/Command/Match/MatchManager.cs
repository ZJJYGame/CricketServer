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
    public class MatchManager:Module<MatchManager>
    {
        /// <summary>
        ///直接加入大池子
        /// </summary>
        public List<MatchDTO> matchSetDict = new List<MatchDTO>();

        public MatchDTO matchSet;
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

                    //var pareams = xRCommon.xRS2CParams();
                    //pareams.Add((byte)ParameterCode.RoleRank, Utility.Json.ToJson(rankDict));
                    //var subOp = xRCommon.xRS2CSub();
                    //subOp.Add((byte)SubOperationCode.Get, pareams);
                    //xRCommon.xRS2CSend(roleId, (byte)ATCmd.SyncRank, (byte)ReturnCode.Success, subOp);
                }
                else
                {
                    Utility.Debug.LogInfo("没有匹配成功！！！");
                }
                //matchSet = matchDto;
            }
            else
            {
                matchDto.RoleId = match.selfData.RoleID;
                matchDto.selfData = match.selfData;
                matchDto.selfCricketData = match.selfCricketData;
                matchDto.otherData = match.otherData;
                matchDto.otherCricketData = match.otherCricketData;
                var setData = matchSetDict.Find(xr => xr.RoleId == match.selfData.RoleID);
                if (setData != null)
                    matchSetDict.Remove(setData);
                matchSetDict.Add(matchDto);
            }
        }

        public void StartMatch()
        { 

        }
    }

}
