using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class RankDTO
    {
        public int RoleID { get; set; }
        public int RoleHeadIcon { get; set; }
        public string RoleName { get; set; }
        public int CricketHeadIcon { get; set; }
        public string CricketName { get; set; }
        public int Duanwei { get; set; }
    }
}
