using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    [CustomeModule]
    public class SpreaCodeManager : Module<SpreaCodeManager>
    {
        public Dictionary<int,int> SpreaCode { get; set; }

        public override void OnPreparatory() => CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncSpreaCode, C2SSpreaCode);


        private void C2SSpreaCode(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            foreach (var item in data)
            {
                switch ((SpreaCodeOperateType)item.Key)
                {
                    case SpreaCodeOperateType.Get:
                        break;
                    case SpreaCodeOperateType.Input:
                        break;
                    case SpreaCodeOperateType.LevelAward:
                        break;
                    case SpreaCodeOperateType.NumAward:
                        break;
                    default:
                        break;
                }
            }
        }




        public void InitCode()
        {
            var obj = NHibernateQuerier.GetTable<SpreaCode>();
            //var codes=  obj.OrderByDescending(o => o.CodeID).ToDictionary(key=>key.CodeID,value=>value.RoleID);
            SpreaCode = obj.OrderByDescending(o => o.CodeID).ToDictionary(key => key.CodeID, value => value.RoleID);
        }
        /// <summary>
        /// 验证邀请码是否存在
        /// </summary>
        /// <param name="codeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public bool VerifyCode(int codeid, out int roleid)
        {
            var result = SpreaCode.TryGetValue(codeid, out roleid);
            if (result)
            {
                return true;
            }
            return false;
        }

        public bool VerifyCode(int codeid)
        {
            var result = SpreaCode.ContainsKey(codeid);
            if (result)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 得到随机邀请码
        /// </summary>
        /// <returns></returns>
        public int RandomCodeID()
        {
            var num = new Random(Guid.NewGuid().GetHashCode()).Next(100000, 999999);
            if (VerifyCode(num))
            {
                RandomCodeID();
            }
            return num;
        }

        public bool AddSpreaCode(int codeid, int roleid)
        {
            return SpreaCode.TryAdd(codeid, roleid);
        }
        /// <summary>
        ///获取玩家邀请信息
        /// </summary>
        /// <param name="roleid"></param>
        public void GetSpreaCode(int roleid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("RoleID", roleid);
            var spreaCode = xRCommon.xRCriteria<SpreaCode>(nHCriteriaRole);


            var roleDict = Utility.Json.ToObject<Dictionary<int,List<int>>>(spreaCode.SpreaLevel);
            var roleList = roleDict.Keys.ToList();
            var roleCickets = new List<RoleCricket>();
            for (int i = 0; i < roleList.Count; i++)
            {
                var nHCriteria=xRCommon.xRNHCriteria("RoleID", roleList[i]);
                roleCickets .Add(xRCommon.xRCriteria<RoleCricket>(nHCriteria));
            }
            //Utility.Debug.LogError("邀请的玩家ID" + Utility.Json.ToJson(roleCickets));
            for (int i = 0; i < roleCickets.Count; i++)
            {
                var crickets = Utility.Json.ToObject<List<int>>(roleCickets[i].CricketList);
                var cricketList = new List<Cricket>();
                for (int j = 0; j < crickets.Count; j++)
                {
                    if (crickets[j] != -1)
                    {
                        var nHCriterias= xRCommon.xRNHCriteria("ID", crickets[j]);
                        cricketList.Add(xRCommon.xRCriteria<Cricket>(nHCriterias));
                    }
                }             
                if (cricketList.Count > 0)
                {
                    var levelList = cricketList.OrderByDescending(o => o.LevelID).ToList();
                    if (levelList[0].LevelID >= 10)
                    {
                        if (roleDict[roleCickets[i].RoleID][0] == -1)
                        {
                            roleDict[roleCickets[i].RoleID][0] = 0;
                        }
                    }

                    if (levelList[0].LevelID >= 20)
                    {
                        if (roleDict[roleCickets[i].RoleID][1] == -1)
                        {
                            roleDict[roleCickets[i].RoleID][1] = 0;
                        }
                    }

                    if (levelList[0].LevelID >= 30)
                    {
                        if (roleDict[roleCickets[i].RoleID][2] == -1)
                        {
                            roleDict[roleCickets[i].RoleID][2] = 0;
                        }
                    }
                }
            }

            Utility.Debug.LogError(Utility.Json.ToJson(roleDict));


        }
        /// <summary>
        /// 输入玩家邀请码
        /// </summary>
        public void InputSpreaCode(int roleid,int codeid)
        {
            var nHCriteriaRole = xRCommon.xRNHCriteria("CodeID", codeid);
            var spreaCode = xRCommon.xRCriteria<SpreaCode>(nHCriteriaRole);

            if (spreaCode!=null)
            {
                spreaCode.SpreaNum += 1;
            }
        }
    }
}
