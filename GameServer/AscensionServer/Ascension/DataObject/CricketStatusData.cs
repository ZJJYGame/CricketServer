using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class CricketStatusData
    {
        public int Level { set; get; }
        public int Atk { set; get; }
        public int Hp { set; get; }
        public int Defense { set; get; }
        public int Mp { set; get; }
        public int MpReply { set; get; }
        public int Crt { set; get; }
        public int CrtAtk { set; get; }
        public int CrtDef { set; get; }
        public int ReduceAtk { set; get; }
        public int ReduceDef { set; get; }
        public int Rebound { set; get; }
        public int Eva { set; get; }
    }
}
