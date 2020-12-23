using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using Protocol;
using System.Collections;

namespace AscensionServer
{
    [CustomeModule]
    public class SpreaCodeManager : Module<SpreaCodeManager>
    {
        public Hashtable SpreaCode { get; set; }
        
        public void GetSET()
        {
         
        }
    }
}
