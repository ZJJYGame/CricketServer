using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class CricketPointMap : ClassMap<CricketPoint>
    {
        public CricketPointMap()
        {
            Id(x => x.CricketID).GeneratedBy.Assigned().Column("cricketid");
            Map(x => x.Str).Column("str");
            Map(x => x.Con).Column("con");
            Map(x => x.Dex).Column("dex");
            Map(x => x.Def).Column("def");
            Map(x => x.FreePoint).Column("free_point");
            Table("cricket_ability_point");
        }
    }
}
