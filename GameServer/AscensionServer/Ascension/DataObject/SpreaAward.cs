using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class SpreaAward
    {
        public int GiftID { get; set; }
        public int Money { get; set; }
        public int Cricket { get; set; }
        public List<int> PropID { get; set; }
        public List<int> PropNumber { get; set; }
        public int AwardState { get; set; }
        public int AwardType { get; set; }
    }
}
