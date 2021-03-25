using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class CricketPointDTO
    {
        public virtual int CricketID { set; get; }
        public virtual int Str { set; get; }
        public virtual int Con { set; get; }
        public virtual int Dex { set; get; }
        public virtual int Def { set; get; }
        public virtual int FreePoint { set; get; }
        public void Clear()
        {
            Str = 0;
            Con = 0;
            Dex = 0;
            Def = 0;
            FreePoint = 0;
        }
    }
}
