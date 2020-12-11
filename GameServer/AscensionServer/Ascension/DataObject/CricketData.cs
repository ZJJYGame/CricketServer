using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class CricketData
    {
        public int CricketID { set; get; }
        public int LevelID { set; get; }
        public int Exp { set; get; }
        public string  CricketName { set; get; }
        public int RankID { set; get; }
    }
}
