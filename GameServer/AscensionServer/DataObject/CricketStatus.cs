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
        public virtual float Atk { set; get; }
        public virtual float Hp { set; get; }
        public virtual float Defense { set; get; }
        public virtual float Mp { set; get; }
        public virtual float MpReply { set; get; }
        public virtual float Crt { set; get; }
        public virtual float CrtAtk { set; get; }
        public virtual float CrtDef { set; get; }
        public virtual float ReduceAtk { set; get; }
        public virtual float ReduceDef { set; get; }
        public virtual float Rebound { set; get; }
        public virtual float Eva { set; get; }
        public virtual double Speed { set; get; }
        public CricketStatus()
        {
            Atk = 0;
            Hp =20;
            Defense = 0;
            Mp = 99;
            MpReply = 40;
            Crt = 0;
            CrtAtk = 50;
            CrtDef = 0;
            ReduceAtk = 0;
            ReduceDef = 0;
            Rebound = 0;
            Eva = 0;
            Speed = 2000;
        }
    }
}
