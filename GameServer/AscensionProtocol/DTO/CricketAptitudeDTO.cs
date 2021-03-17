using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
   public class CricketAptitudeDTO
    {
        public virtual int CricketID { get; set; }
        public virtual int Str { get; set; }
        public virtual int Con { get; set; }
        public virtual int Dex { get; set; }
        public virtual int Def { get; set; }
        public virtual int SkillStr { get; set; }
        public virtual int SkillCon { get; set; }
        public virtual int SkillDex { get; set; }
        public virtual int SkillDef { get; set; }
        public virtual int StrAptitude { get; set; }
        public virtual int ConAptitude { get; set; }
        public virtual int DexAptitude { get; set; }
        public virtual int DefAptitude { get; set; }

    }
}
