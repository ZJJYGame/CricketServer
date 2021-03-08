﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    public class BattleCombat
    {
        public virtual int RoleID { get; set; }
        public virtual int MatchWon { get; set; }
        public virtual int MoneyLimit { get; set; }

        public BattleCombat()
        {
            RoleID = -1;
            MatchWon = 0;
            MoneyLimit = 0;
        }
    }
}