using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public  class HeadPortraitData
    {
        public int PlayerHeadID { get; set; }
        public string PlayerHead { get; set; }
    }
}
