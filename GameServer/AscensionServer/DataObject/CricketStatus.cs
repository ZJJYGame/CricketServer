using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public class CricketStatus
    {
        public virtual int CricketID { set; get; }
        public virtual int Atk { set; get; }
        public virtual int Hp { set; get; }
        public virtual int Defense { set; get; }
        public virtual int Mp { set; get; }
        public virtual int MpReply { set; get; }
        public virtual int Crt { set; get; }
        public virtual int CrtAtk { set; get; }
        public virtual int CrtDef { set; get; }
        public virtual int ReduceAtk { set; get; }
        public virtual int ReduceDef { set; get; }
        public virtual int Rebound { set; get; }
        public virtual int Eva { set; get; }
        public virtual int Speed { set; get; }
        public CricketStatus()
        {
            Atk = 10;
            Hp =30;
            Defense = 10;
            Mp = 100;
            MpReply = 10;
            Crt = 0;
            CrtAtk = 150;
            CrtDef = 0;
            ReduceAtk = 0;
            ReduceDef = 0;
            Rebound = 0;
            Eva = 0;
            Speed = 2000;
        }
    }
}
