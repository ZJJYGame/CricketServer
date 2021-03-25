using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    class BattleCombatMap:ClassMap<BattleCombat>
    {
        public BattleCombatMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.MatchWon).Column("matchwon");
            Map(x => x.MoneyLimit).Column("moneylimit");
            Map(x => x.RoleName).Column("roleName");
            Table("battlecombat");
        }
    }
}
