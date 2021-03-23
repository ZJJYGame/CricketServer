using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public  class CricketHeadPortraitData
    {
        public int CricketID { set; get; }
        public string CricketHead { set; get; }
        public string CricketModel { set; get; }
    }
}
