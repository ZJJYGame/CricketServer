using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SpreaCode
    {
        public virtual int RoleID { get; set; }
        public virtual int CodeID { get; set; }
        public virtual int SpreaNum { get; set; }
        public virtual string SpreaPlayers { get; set; }

        public SpreaCode()
        {
            CodeID = 0;
            SpreaNum = 0;
            SpreaPlayers = "{}";
        }
    }
}
