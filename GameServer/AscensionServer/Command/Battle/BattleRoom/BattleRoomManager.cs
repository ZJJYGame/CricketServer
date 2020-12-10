using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Cosmos.Reference;
using AscensionProtocol;
using Protocol;


namespace AscensionServer
{
    [CustomeModule]
    public class BattleRoomManager:Module<BattleRoomManager>
    {
        #region json数据
        private Dictionary<int, BattleBuffData> battlebuffDataDict;
        public Dictionary<int,BattleBuffData> BattleBuffDataDict
        {
            get
            {
                if (battlebuffDataDict == null)
                {
                    GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleBuffData>>(out var tempDict);
                    battlebuffDataDict = tempDict;
                }
                return battlebuffDataDict;
            }
        }
        #endregion

        //战斗房间id起始位置
        int battleRoomIdStartIndex = 1000;
        //回收后可被使用的房间Id列表
        List<int> canUseRoomIdList=new List<int>();
        //占用中的房间Id列表
        List<int> occupiedRoomIdList = new List<int>();


        public Random random = new Random();

        /// 战斗房间实体字典，key=>房间Id,value=>房间实体对象
        Dictionary<int, BattleRoomEntity> battleRoomEntityDict=new Dictionary<int, BattleRoomEntity>();

        public override void OnInitialization()
        {
            CommandEventCore.Instance.AddEventListener((ushort)ATCmd.SyncBattle, EnterBattleHandler);
        }
        public override void OnPreparatory()
        {
            //Utility.Debug.LogInfo("开始创建房间");
            //CreateRoom(111, 222);
        }

        void EnterBattleHandler(OperationData OpData)
        {
            Utility.Debug.LogError("收到战斗请求=>"+ OpData.DataMessage.ToString());
            Dictionary<byte, object> tempDict = Utility.Json.ToObject<Dictionary<byte, object>>(OpData.DataMessage.ToString());
            int roleId = Convert.ToInt32(tempDict[(byte)ParameterCode.RoleCricket]);
            CreateRoom(roleId, 111);
        }
        public void S2CEnterBattle(int roleid,List<BattleRoleActionData> battleRoleActionDataList)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = Utility.Json.ToJson(battleRoleActionDataList);
            operationData.OperationCode = (ushort)ATCmd.SyncBattle;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, operationData);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom(int playerOne_Id,int playerTwo_Id)
        {
            BattleRoomEntity battleRoomEntity = GameManager.ReferencePoolManager.Spawn<BattleRoomEntity>();
            int roomId = GetRoomId();
            battleRoomEntity.Init(roomId,playerOne_Id, playerTwo_Id);
        }
        /// <summary>
        /// 销毁房间
        /// </summary>
        public void DestoryRoom(int roomId)
        {
            if (battleRoomEntityDict.ContainsKey(roomId))
            {
                occupiedRoomIdList.Remove(roomId);
                canUseRoomIdList.Add(roomId);
                GameManager.ReferencePoolManager.Despawn(battleRoomEntityDict[roomId]);
            }
            battleRoomEntityDict.Remove(roomId);
        }

        /// <summary>
        /// 获取可使用的roomId
        /// </summary>
        /// <returns></returns>
        int GetRoomId()
        {
            int roomId;
            if (canUseRoomIdList.Count > 0)
            {
                roomId = canUseRoomIdList[0];
            }
            else
            {
                roomId = battleRoomIdStartIndex;
                battleRoomIdStartIndex++;
            }
            return roomId;
        }
    }
}
