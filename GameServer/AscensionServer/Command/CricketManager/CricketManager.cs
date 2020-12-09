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
    public class CricketManager : Module<CricketManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncCricket, C2SCricket);
        }


        public void C2SCricket(OperationData opData)
        {
            var data = Utility.Json.ToObject<Dictionary<byte,string>>(opData.DataMessage.ToString());
            Utility.Debug.LogInfo("yzqData请求蛐蛐属性:" +Utility.Json.ToJson(data));
            foreach (var item in data)
            {
                var dict = Utility.Json.ToObject<Dictionary<byte, string>>(item.Value);
                switch ((CricketOperateType)item.Key)
                {
                    case CricketOperateType.AddCricket:
                        break;
                    case CricketOperateType.GetCricket:
                        var role = Utility.Json.ToObject<Role>(dict[(byte)ParameterCode.RoleCricket]);
                        Utility.Debug.LogInfo("yzqData请求蛐蛐属性:" + role.RoleID);
                        RoleCricketManager.GetRoleCricket(role.RoleID);
                        break;
                    case CricketOperateType.GetCricketStatus:
                        break;
                    case CricketOperateType.RemoveCricket:
                        break;
                    case CricketOperateType.AddPoint:
                        var roleObj = Utility.Json.ToObject<Role>(dict[(byte)ParameterCode.Role]);
                        var pointObj = Utility.Json.ToObject<CricketPointDTO>(dict[(byte)ParameterCode.CricketPoint]);
                        Utility.Debug.LogInfo("yzqData请求蛐蛐属性:" + roleObj.RoleID);
                        RoleCricketManager.AddPointForScricket(roleObj.RoleID, pointObj.CricketID, pointObj);
                        break;
                    case CricketOperateType.ResetPoint:
                        break;
                    case CricketOperateType.LevelUP:
                        break;
                    case CricketOperateType.UseItem:
                        break;
                    default:
                        break;
                }
            }


        }

        public void S2CCricketMessage(int roleid,string message,ReturnCode returnCode)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = message;
            operationData.ReturnCode = (byte)returnCode;
            operationData.OperationCode = (ushort)ATCmd.SyncCricket;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, operationData);
        }
    }
}
