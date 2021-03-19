using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class ExplorationDTO
    {
        public int RoleID { get; set; }
        public Dictionary<int, ExplorationItemDTO> ExplorationItemDict { get; set; }
        /// <summary>
        /// 判断是否解锁
        /// </summary>
        public Dictionary<int,bool> UnLockDict { get; set; }

        public Dictionary<int,int> CatchPropAndTimeProp { get; set; }
    }
    [Serializable]
    public class ExplorationItemDTO
    {
        //这是物品ID 对应数量
        public Dictionary<int,int> ItemId { get; set; }
        /// <summary>
        /// 获取当前事件的时间  方便倒计时 
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// 时间类型
        /// </summary>
        public int TimeType { get; set; }
        /// <summary>
        /// 自定义蛐蛐id
        /// </summary>
        public int CustomId { get; set; }
        /// <summary>
        /// 全局蛐蛐id
        /// </summary>
        public int GlobalId { get; set; }
    }
}
