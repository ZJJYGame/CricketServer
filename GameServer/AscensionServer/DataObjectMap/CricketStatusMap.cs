using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class CricketStatusMap : ClassMap<CricketStatus>
    {
        public CricketStatusMap()
        {
            Id(x => x.CricketID).GeneratedBy.Assigned().Column("cricketid");
            Map(x => x.Crt).Column("crt");
            Map(x => x.CrtAtk).Column("crtatk");
            Map(x => x.CrtDef).Column("crtdef");
            Map(x => x.Defense).Column("defense");
            Map(x => x.Eva).Column("eva");
            Map(x => x.Hp).Column("hp");
            Map(x => x.Mp).Column("mp");
            Map(x => x.MpReply).Column("mpreply");
            Map(x => x.Rebound).Column("rebound");
            Map(x => x.ReduceAtk).Column("reduceatk");
            Map(x => x.ReduceDef).Column("reducedef");
            Map(x => x.Atk).Column("atk");
            Table("cricket_status");

        }
    }
}
