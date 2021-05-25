using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum TowerOpCode:byte
    {
        //获取爬塔的信息
        GetTowerData=0,
        //放弃挑战
        Abandon=1,
        //开始挑战
        StartBattle=2,
        ChooseDifficulty=3,
    }
}
