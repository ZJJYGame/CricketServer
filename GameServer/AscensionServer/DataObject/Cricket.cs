using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public class Cricket
    {
        public virtual int CricketID { set; get; }
        public virtual int Level { set; get; }
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
    }
}
