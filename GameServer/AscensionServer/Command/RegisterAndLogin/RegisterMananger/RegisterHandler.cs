﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol;
using Cosmos.Reference;
namespace AscensionServer
{
    public static  class RegisterHandler
    {
        public static void RegisterRole(string account,string password,object peer)
        {
            NHCriteria nHCriteriaAccount = xRCommon.xRNHCriteria("Account", account);
            Utility.Debug.LogInfo("yzqData发送失败" + nHCriteriaAccount.Value.ToString());

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketStatus>>(out var cricketStatusDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, Cricket>>(out var cricketDict);
            bool isExist = xRCommon.xRVerify<User>(nHCriteriaAccount);

            var userObj = new User() {Account= account,Password= password };
            var role = new Role() { };
            var roleAsset = new RoleAssets();
            var cricket = new Cricket();
            var roleCricketObj = new RoleCricketDTO();
            var roleCricket = new RoleCricket();
            var cricketStatus = new CricketStatus();
            var cricketAptitude = new CricketAptitude();
            var cricketPoint = new CricketPoint();
            var cricketAddition = new CricketAddition();
            if (!isExist)
            {
                userObj = NHibernateQuerier.Insert(userObj);
                NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);

                role = NHibernateQuerier.Insert(role);
                userObj.RoleID = role.RoleID;
                NHibernateQuerier.Update(userObj);
                roleAsset.RoleID = role.RoleID;
                NHibernateQuerier.Insert(roleAsset);
                #region 待换
                cricket = NHibernateQuerier.Insert(cricket);
                roleCricketObj.CricketList[0] = cricket.ID;
                roleCricket.RoleID = role.RoleID;
                roleCricket.CricketList = Utility.Json.ToJson(roleCricketObj.CricketList);
                roleCricket.TemporaryCrickets = Utility.Json.ToJson(roleCricketObj.TemporaryCrickets);
                NHibernateQuerier.Insert(roleCricket);
                cricketStatus.CricketID = cricket.ID;
                NHibernateQuerier.Insert(cricketStatus);
                cricketAptitude.CricketID = cricket.ID;
                NHibernateQuerier.Insert(cricketAptitude);
                cricketPoint.CricketID = cricket.ID;
                NHibernateQuerier.Insert(cricketPoint);
                cricketAddition.CricketID = cricket.ID;
                NHibernateQuerier.Insert(cricketAddition);
                #endregion
                #region 插入背包和每日任务 以及探索
                NHibernateQuerier.Insert(new Inventory() { RoleID = role.RoleID });
                Dictionary<int, TaskItemDTO> darilyDict = new Dictionary<int, TaskItemDTO>();
                darilyDict.Add(301, new TaskItemDTO() { taskStatus = false, taskProgress = 0, taskTarget = 1, taskManoy = 90 });
                darilyDict.Add(302, new TaskItemDTO() { taskStatus = false, taskProgress = 0, taskTarget = 2, taskManoy = 140 });
                darilyDict.Add(303, new TaskItemDTO() { taskStatus = false, taskProgress = 0, taskTarget = 3, taskManoy = 140 });
                NHibernateQuerier.Insert(new xRTask() { RoleID = role.RoleID, taskDict = Utility.Json.ToJson(darilyDict) });
                NHibernateQuerier.Insert(new Exploration() { RoleID = role.RoleID });
                #endregion
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(role);
                operationData.ReturnCode = (byte)ReturnCode.Success;
                operationData.OperationCode = (ushort)ATCmd.Register;
                Utility.Debug.LogInfo("yzqData发送成功");
                GameManager.CustomeModule<PeerManager>().SendMessage((peer as IPeerEntity).SessionId, operationData);
            }
            else
            {
                OperationData operationData = new OperationData();
                operationData.DataMessage = "账号已存在";
                operationData.ReturnCode = (byte)ReturnCode.Fail;
                operationData.OperationCode = (ushort)ATCmd.Register;
                Utility.Debug.LogInfo("yzqData发送失败");
                GameManager.CustomeModule<PeerManager>().SendMessage((peer as IPeerEntity).SessionId, operationData);
            }
        }

  
    }
}
