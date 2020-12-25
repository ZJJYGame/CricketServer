using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using Protocol;

namespace AscensionServer
{
    [CustomeModule]
    public partial class SpreaCodeManager : Module<SpreaCodeManager>
    {
        public Dictionary<int,int> SpreaCode { get; set; }

        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncSpreaCode, C2SSpreaCode);
            InitCode();
        }

        private void C2SSpreaCode(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte, object>>(opData.DataMessage.ToString());
            foreach (var item in data)
            {
                var spreacode = Utility.Json.ToObject<SpreaCodeDTO>(item.Value.ToString());
                switch ((SpreaCodeOperateType)item.Key)
                {
                    case SpreaCodeOperateType.Get:
                        Utility.Debug.LogInfo("1YZQ" + Utility.Json.ToJson(item));
                        GetSpreaCode(spreacode.RoleID);
                        break;
                    case SpreaCodeOperateType.Input:
                        Utility.Debug.LogInfo("2YZQ"+ Utility.Json.ToJson(item)); 
                        InputSpreaCode(spreacode.RoleID, spreacode.CodeID);
                        break;
                    case SpreaCodeOperateType.LevelAward:
                        ReceiveLevelAwar(spreacode);
                        break;
                    case SpreaCodeOperateType.NumAward:
                        ReceiveNumAward(spreacode);
                        break;
                    default:
                        break;
                }
            }
        }

        public  void InitCode()
        {
            var obj = NHibernateQuerier.GetTable<SpreaCode>();
            //var codes=  obj.OrderByDescending(o => o.CodeID).ToDictionary(key=>key.CodeID,value=>value.RoleID);
            SpreaCode = obj.OrderByDescending(o => o.CodeID).ToDictionary(key => key.CodeID, value => value.RoleID);
            Utility.Debug.LogError("YZQ获得的数据库邀请码"+ Utility.Json.ToJson(SpreaCode));
        }
        /// <summary>
        /// 验证邀请码是否存在
        /// </summary>
        /// <param name="codeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public  bool VerifyCode(int codeid, out int roleid)
        {
            var result = SpreaCode.TryGetValue(codeid, out roleid);
            if (result)
            {
                return true;
            }
            return false;
        }

        public  bool VerifyCode(int codeid, int roleid)
        {
            {
                return SpreaCode.TryAdd(codeid, roleid);
            }
        }
        /// <summary>
        /// 得到随机邀请码
        /// </summary>
        /// <returns></returns>
        public  int RandomCodeID(int roleid)
        {
            var num = new Random(Guid.NewGuid().GetHashCode()).Next(100000, 999999);
            if (VerifyCode(num, roleid))
            {
                RandomCodeID(roleid);
            }
            return num;
        }

        

    }
}
