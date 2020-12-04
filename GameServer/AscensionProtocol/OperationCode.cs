using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
        Default =0,
        Login = 1,
        Logoff=2,
        Register=3,
        /// <summary>
        /// 同步当前这个角色的数据
        /// </summary>
        SyncRole=4,
        /// <summary>
        /// 同步当前角色的位置信息，position&rotation 
        /// </summary>
        PlayerInputCommand = 5,
    /// <summary>
    /// 登录角色
    /// </summary>
        LoginRole=6,
      
        /// <summary>
        /// 测试消息队列
        /// </summary>
        MessageQueue = 187,
        /// <summary>
        /// 网关token
        /// </summary>
        Token=243,
        /// <summary>
        /// 心跳
        /// </summary>
        HeartBeat = 244,
        /// <summary>
        /// 子操作码
        /// </summary>
        SubOpCodeData = 254,
        SubOperationCode = 255
         
    }
}
