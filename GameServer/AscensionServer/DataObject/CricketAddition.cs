﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public  class CricketAddition
    {
        public virtual int CricketID { set; get; }
        public virtual int Atk { set; get; }
        public virtual int Hp { set; get; }
        public virtual int Defense { set; get; }
        public virtual int Mp { set; get; }
        public virtual int MpReply { set; get; }      
    }
}
