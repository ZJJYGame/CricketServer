using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.RoleID).GeneratedBy.Increment().Column("roleid");
            Map(x => x.RoleName).Column("role_name");
            Table("role");

        }
    }
}
