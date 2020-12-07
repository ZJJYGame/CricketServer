using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleAssetsMap : ClassMap<RoleAssets>
    {
        public RoleAssetsMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.RoleGold).Column("role_gold");
            Table("role_assets");

        }

    }
}
