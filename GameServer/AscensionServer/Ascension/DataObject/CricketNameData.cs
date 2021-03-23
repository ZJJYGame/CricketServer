using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class CricketNameData
    {
        public int NameID { set; get; }
        public string CricketName { set; get; }
    }
}
