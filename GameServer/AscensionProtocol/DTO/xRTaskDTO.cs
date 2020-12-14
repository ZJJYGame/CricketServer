using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class TaskDTO
    {
        public int RoleID { get; set; }
        public Dictionary<int, TaskItemDTO> taskDict { get; set; }
    }
    [Serializable]
    public class TaskItemDTO
    {
        public int taskTarget { get; set; }
        public int taskProgress { get; set; }
        public bool taskStatus { get; set; }
        public int taskManoy { get; set; }
    }
}
