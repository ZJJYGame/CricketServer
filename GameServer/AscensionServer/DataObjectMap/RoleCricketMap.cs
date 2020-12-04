using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class RoleCricketMap:ClassMap<RoleCricket>
    {
        public RoleCricketMap()
        {
            Id(x => x.RoleID).GeneratedBy.Increment().Column("role_id");
            Map(x => x.RoleCrickets).Column("role_cricket");
            Table("role_cricket");
        }
    }
}
