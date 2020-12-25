using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class MatchDTO
    {
        public int RoleId { get; set; }
        public RoleDTO selfData { get; set; }
        public CricketDTO selfCricketData { get; set; }
        public RoleDTO otherData { get; set; }
        public CricketDTO otherCricketData { get; set; }
    }
}
