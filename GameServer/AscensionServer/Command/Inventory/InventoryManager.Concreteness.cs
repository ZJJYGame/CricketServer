﻿using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
    public partial class InventoryManager
    {

        /// <summary>
        /// 映射T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nHCriteria"></param>
        /// <returns></returns>
        public static T xRCriteriaSelectMethod<T>(NHCriteria nHCriteria)
        {
            return NHibernateQuerier.CriteriaSelect<T>(nHCriteria);
        }
        /// <summary>
        ///映射 NHCriteria
        /// </summary>
        /// <param name="customs"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static NHCriteria xRNHCriteria(string customs, object values)
        {
            return GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue(customs, values);
        }
        /// <summary>
        /// 验证的映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nHCriteria"></param>
        /// <returns></returns>
        public static bool xRVerify<T>(NHCriteria nHCriteria)
        {
            return  NHibernateQuerier.Verify<T>(nHCriteria);
        }
        /// <summary>
        /// 统一参数
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte,object> xRS2CParams()
        {
            Dictionary<byte, object> paramsS2CDict = new Dictionary<byte, object>();
            return paramsS2CDict;
        }
        /// <summary>
        /// 统一sub码
        /// </summary>
        /// <returns></returns>
        public static Dictionary<byte, object> xRS2CSub()
        {
            Dictionary<byte, object> subS2CDict = new Dictionary<byte, object>();
            return subS2CDict;
        }

        /// <summary>
        /// 统一发送
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="op"></param>
        public static void xRS2CSend(int roleId,ushort op)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = op;
            opData.DataMessage = xRS2CSub();
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
        }


        public static void xRGetInventory(int roleId)
        {
            var nHcriteria = xRNHCriteria("RoleID", roleId);
            if (xRVerify<Role>(nHcriteria))
            {
                var xRserver = xRCriteriaSelectMethod<Inventory>(nHcriteria);
                Utility.Debug.LogInfo("老陆==>" + xRserver.ItemDict);
                xRS2CParams().Add((byte)ParameterCode.RoleInventory, xRserver.ItemDict);
                xRS2CSub().Add((byte)subInventoryOp.Get, xRS2CParams());
                xRS2CSend(roleId, (byte)ATCmd.SyncInventory);
            }
        }

    }
}
