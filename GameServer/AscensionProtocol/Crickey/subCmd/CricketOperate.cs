using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum CricketOperateType:byte
    {
        None=0,
        AddCricket=1,
        GetCricket = 2,
        GetTempCricket=3,
        RemoveCricket = 4,
        AddPoint=5,
        ResetPoint = 6,
        LevelUP=7,
        UseItem=8,
    }
}
