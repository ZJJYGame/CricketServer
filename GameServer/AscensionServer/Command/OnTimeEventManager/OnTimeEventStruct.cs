using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 准点事件结构体
    /// </summary>
    public struct OnTimeEventStruct
    {
        public int hours;
        public int minutes;
        public int seconds;
        /// <summary>
        /// 一周哪几天触发（如周一，周二），从0~6
        /// </summary>
        public int[] dayInWeek;
        /// <summary>
        /// 触发的回调函数
        /// </summary>
        public Action<string> actionCallBack;
        public OnTimeEventStruct(int hours,int minutes,int seconds,int[] dayInWeek,Action<string> action)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.dayInWeek = dayInWeek;
            this.actionCallBack = action;
        }
    }
}
