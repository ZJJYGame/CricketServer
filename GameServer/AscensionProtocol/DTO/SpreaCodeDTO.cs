using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
   public class SpreaCodeDTO
    {
        public virtual int CodeID { get; set; }
        public virtual int RoleID { get; set; }
        public virtual int SpreaNum { get; set; }
        public virtual int AwardType { get; set; }
        /// <summary>
        /// 玩家名字为key对应三个等级宝箱-1,0,1分别对应领取状态未获得，未领取，已领取
        /// </summary>
        public virtual Dictionary<int, List<int>> SpreaLevel { get; set; }
        public virtual Dictionary<int, int> SpreaPlayers  { get; set; }
    }
}
