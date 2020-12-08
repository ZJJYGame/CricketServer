using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [ConfigData]
    [Serializable]
    public class BattleBuffData
    {
        public int buffId;
        public BattleBuffEffectProperty buffEffectProperty;
    }
    public enum BattleBuffEffectProperty:byte
    {
        Attack=0,
        Defense=1,
        Endurance=2,
        EnduranceReply=3,
        CritProp=4,
        ReduceDamage=5,
        Pierce=6,
        ReboundDamage=7,
        CritDamage=8,
        CritResistance=9,
    }
}
