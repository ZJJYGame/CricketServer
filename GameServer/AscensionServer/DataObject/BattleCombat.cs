using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class BattleCombat:IComparable<BattleCombat>
    {
        public virtual int RoleID { get; set; }
        public virtual int MatchWon { get; set; }
        public virtual int MoneyLimit { get; set; }
     //   public virtual string RoleName { get; set; }

        public BattleCombat()
        {
            RoleID = -1;
            MatchWon = 0;
            MoneyLimit = 0;
          //  RoleName = "";
        }

        public virtual int CompareTo(BattleCombat other)
        {
            if (MatchWon == other.MatchWon)
                return 0;
            else if (MatchWon > other.MatchWon)
                return -1;
            else
                return 1;
        }
    }
}
