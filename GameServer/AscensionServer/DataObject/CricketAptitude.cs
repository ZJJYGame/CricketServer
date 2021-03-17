using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class CricketAptitude
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

        public CricketAptitude()
        {
            Str = 10;
            Con = 10;
            Dex = 10;
            Def = 10;
            SkillStr = 0;
            SkillCon = 0;
            SkillDex = 0;
            SkillDef = 0;
            StrAptitude = 50;
            ConAptitude = 50;
            DexAptitude = 50;
            DefAptitude = 50;
        }
    }
}
