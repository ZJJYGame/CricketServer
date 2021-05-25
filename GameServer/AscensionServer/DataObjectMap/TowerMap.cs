using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class TowerMap:ClassMap<Tower>
    {
        public TowerMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.MaxDifficulty).Column("maxdifficulty");
            Map(x => x.NowChooseDifficulty).Column("nowchoosedifficulty");
            Map(x => x.NowLevel).Column("nowlevel");
        }
    }
}
