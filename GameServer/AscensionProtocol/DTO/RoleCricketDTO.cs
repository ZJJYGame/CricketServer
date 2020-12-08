using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public  class RoleCricketDTO
    {
        public virtual int RoleID { set; get; }
        public virtual Dictionary<int, int> CricketList { set; get; }
        public virtual Dictionary<int,int> TemporaryCrickets { get; set; }

        public RoleCricketDTO()
        {
            CricketList = new Dictionary<int, int>();
            CricketList.Add(0,0);
            CricketList.Add(1, 0);
            CricketList.Add(2, 0);
            TemporaryCrickets = new Dictionary<int, int>();
            TemporaryCrickets.Add(0, 0);
            TemporaryCrickets.Add(1, 0);
            TemporaryCrickets.Add(2, 0);
        }
    }
}
