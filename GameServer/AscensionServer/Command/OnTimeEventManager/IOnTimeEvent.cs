using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 计时执行事件接口
    /// </summary>
    public interface  IOnTimeEvent
    {
         void OnTimeEventHandler();
    }
}
