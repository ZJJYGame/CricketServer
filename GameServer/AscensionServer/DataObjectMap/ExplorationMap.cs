using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class ExplorationMap:ClassMap<Exploration>
    {
        public ExplorationMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.ExplorationItemDict).Column("explorationitemdict");
            Map(x => x.UnLockDict).Column("unlockDict");
            Map(x => x.CatchAndTimeDict).Column("catchandtimedict");
            Table("exploration");
        }
    }
}
