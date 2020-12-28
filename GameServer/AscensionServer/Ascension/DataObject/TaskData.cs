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
        public string TaskType { get; set; }
        public List<int> TaskMoney { get; set; }
        public int PropID { get; set; }
        public bool TaskState { get; set; }
        public int TaskNow { get; set; }
        public int TaskTarget { get; set; }
        public int TaskMoneySet { get; set; }
    }
}
