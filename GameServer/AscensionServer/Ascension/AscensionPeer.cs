﻿using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Cosmos;
using System;
using System.Text;
using Protocol;
using System.ServiceModel.Configuration;

namespace AscensionServer
{
    public class AscensionPeer : ClientPeer, IPeerEntity
    {
        #region Properties
        public int SessionId { get; private set; }
        public bool Available { get; private set; }
        public object Handle { get; private set; }
        SendParameters sendParam = new SendParameters();
        EventData eventData = new EventData();
        public RoleEntity RoleEntity { get; set; }
        Dictionary<Type, object> dataDict = new Dictionary<Type, object>();
        #endregion
        #region Methods
        public AscensionPeer(InitRequest initRequest) : base(initRequest)
        {
            Handle = this; this.SessionId = ConnectionId;
            GameManager.CustomeModule<PeerManager>().TryAdd(this);
            Utility.Debug.LogInfo($"Photon SessionId : {SessionId} Available . RemoteAdress:{initRequest.RemoteIP}");
        }
        /// <summary>
        /// 发送消息；传输的数据类型；用户自定义数据
        /// </summary>
        /// <param name="opData"></param>
        public void SendMessage(OperationData opData)
        {
            var data = Utility.Json.ToJson(opData);
            base.SendMessage(data, sendParam);
        }
        /// <summary>
        /// 发送事件消息;
        /// 传输的数据类型限定为Dictionary<byte,object>类型；
        /// </summary>
        /// <param name="data">用户自定义数据</param>
        public void SendEventMsg(byte opCode, Dictionary<byte, object> data)
        {
            eventData.Code = opCode;
            eventData.Parameters = data;
            base.SendEvent(eventData, sendParam);
        }
        public void Clear()
        {
            SessionId = 0;
            Available = false;
        }
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            EventData ed = new EventData((byte)EventCode.DeletePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            ed.Parameters = data;
            //尝试获取负载的角色数据；
            //if (RoleEntity!=null)
            if (TryGetValue(typeof(RoleEntity), out var roleEntity))
            {
                (roleEntity as RoleEntity).OnDisconnect();
                Utility.Debug.LogError(Utility.Json.ToJson(roleEntity));
                //若存在，则广播到各个模块；
                var opData = new OperationData();
                opData.OperationCode = ProtocolDefine.OPR_PLYAER_LOGOFF;
                opData.DataMessage = roleEntity;
                //opData.DataMessage = (roleEntity as RoleEntity).RoleId;
                Utility.Debug.LogInfo("yzqData获取移除RoleID:"+ (roleEntity as RoleEntity).RoleId);
                var t = CommandEventCore.Instance.DispatchAsync(ProtocolDefine.OPR_PLYAER_LOGOFF, opData);
            }
            GameManager.CustomeModule<PeerManager>().TryRemove(SessionId);
            Utility.Debug.LogError($"Photon SessionId : {SessionId} Unavailable . RemoteAdress:{RemoteIPAddress}");
            var task = GameManager.CustomeModule<PeerManager>().BroadcastEventToAllAsync((byte)reasonCode, ed.Parameters);
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            operationRequest.Parameters.Add((byte)ParameterCode.ClientPeer, this);
            object responseData = GameManager.CustomeModule<NetworkManager>().EncodeMessage(operationRequest);
            var op = responseData as OperationResponse;
            op.OperationCode = operationRequest.OperationCode;
            SendOperationResponse(op, sendParameters);
        }
        /// <summary>
        /// 接收到客户端消息；
        /// </summary>
        protected override void OnMessage(object message, SendParameters sendParameters)
        {
            Utility.Debug.LogInfo(message);
            //接收到客户端消息后，进行委托广播；
            var opData = Utility.Json.ToObject<OperationData>(Convert.ToString( message ));
            opData.DataContract .Messages.Add((byte)ParameterCode.ClientPeer, this);
            CommandEventCore.Instance.Dispatch(opData.OperationCode, opData);

        }

        public bool TryGetValue(Type key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryRemove(Type Key)
        {
            return dataDict.Remove(Key, out _);
        }
        public bool TryRemove(Type key, out object value)
        {
            return dataDict.Remove(key, out value);
        }
        public bool TryAdd(Type key, object Value)
        {
            return dataDict.TryAdd(key, Value);
        }
        public bool TryUpdate(Type key, object newValue, object comparsionValue)
        {
            if (dataDict.ContainsKey(key))
            {
                var equal = dataDict[key].Equals(comparsionValue);
                if (equal)
                    dataDict[key] = newValue;
                return equal;
            }
            return false;
        }
        #endregion
    }
}