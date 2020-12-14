using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class CricketDTO
    {
        public virtual int ID { set; get; }
        public virtual int CricketID { set; get; }
        public virtual int LevelID { set; get; }
        public virtual int Exp { set; get; }
        public virtual string CricketName { set; get; }
        public virtual int RankID { set; get; }
        public virtual Dictionary<int,int> SkillDict { set; get; }
        public virtual Dictionary<int, int> SpecialDict { set; get; }
    }
}
