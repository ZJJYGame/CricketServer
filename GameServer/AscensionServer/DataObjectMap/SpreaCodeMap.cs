using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class SpreaCodeMap : ClassMap<SpreaCode>
    {
        public SpreaCodeMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.CodeID).Column("codeid");
            Map(x => x.SpreaNum).Column("spreanum");
            Map(x => x.SpreaLevel).Column("sprealevel");
            Map(x => x.SpreaPlayers).Column("spreaplayers");
            Table("spreacode");
        }
    }
}
