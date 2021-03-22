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
   public class EigeneRoleInfoManager : Module<EigeneRoleInfoManager>
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.EigeneInfo, C2SEigeneInfo);
        }

        public void C2SEigeneInfo(OperationData opData)
        {
            Utility.Debug.LogInfo("yzqData接收更改名称头像成功" + Utility.Json.ToJson(opData.DataMessage));
            var role = Utility.Json.ToObject<Role>(opData.DataMessage.ToString());
            switch ((EigeneRoleInfoOpCode)opData.SubOperationCode)
            {
                case EigeneRoleInfoOpCode.Rename:
                    RenameS2C(role);
                    break;
                case EigeneRoleInfoOpCode.ReplaceHeadPortrait:
                    ReplaceHeadPortraitS2C(role);
                    break;
                default:
                    break;
            }
        }

        void ReplaceHeadPortraitS2C(Role role)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, HeadPortraitData>>(out var HeadPortraitDict);
            var result = HeadPortraitDict.TryGetValue(role.HeadPortrait,out var headPortraitData);
            if (result)
            {
                NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", role.RoleID);
                var roleObj = xRCommon.xRCriteria<Role>(nHCriteria);
                if (roleObj != null)
                {
           
                    roleObj.HeadPortrait = headPortraitData.PlayerHeadID;
                    NHibernateQuerier.Update(roleObj);
                    Utility.Debug.LogInfo("yzqData更改名称头像成功" + Utility.Json.ToJson(roleObj));

                    OperationData opData = new OperationData();
                    opData.OperationCode = (ushort)ATCmd.EigeneInfo;
                    opData.SubOperationCode = (byte)EigeneRoleInfoOpCode.ReplaceHeadPortrait;
                    opData.ReturnCode = (byte)ReturnCode.Success;
                    opData.DataMessage = Utility.Json.ToJson(roleObj);
                    GameManager.CustomeModule<RoleManager>().SendMessage(role.RoleID, opData);
                }else
                    xRCommon.xRS2CSend(role.RoleID, (ushort)ATCmd.EigeneInfo, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAssets);
            }else
                xRCommon.xRS2CSend(role.RoleID, (ushort)ATCmd.EigeneInfo, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAssets);

        }


        void RenameS2C(Role role)
        {
            NHCriteria nHCriteria = xRCommon.xRNHCriteria("RoleID", role.RoleID);
            var roleObj = xRCommon.xRCriteria<Role>(nHCriteria);
            if (roleObj != null)
            {
                roleObj.RoleName = role.RoleName;
                NHibernateQuerier.Update(roleObj);
                OperationData opData = new OperationData();
                opData.OperationCode = (ushort)ATCmd.EigeneInfo;
                opData.SubOperationCode = (byte)EigeneRoleInfoOpCode.Rename;
                opData.ReturnCode = (byte)ReturnCode.Success;
                opData.DataMessage = Utility.Json.ToJson(roleObj);
                GameManager.CustomeModule<RoleManager>().SendMessage(role.RoleID, opData);
            }
            else
            {
                xRCommon.xRS2CSend(role.RoleID, (ushort)ATCmd.EigeneInfo, (byte)ReturnCode.Fail, xRCommonTip.xR_err_VerifyAssets);
            }

        }



    }
}
