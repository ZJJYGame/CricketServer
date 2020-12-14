using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class TaskData
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescribe { get; set; }
        public List<int> TaskMoney { get; set; }
        public int TaskPropId { get; set; }
        public int TaskState { get; set; }
    }
}
