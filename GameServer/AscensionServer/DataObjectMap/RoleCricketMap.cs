using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleCricketMap : ClassMap<RoleCricket>
    {
        public RoleCricketMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.CricketList).Column("cricketlist");
            Map(x => x.TemporaryCrickets).Column("temporary_crickets");           
            Table("role_cricket");
        }
    }
}
