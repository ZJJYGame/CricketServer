using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class CricketAdditionMap : ClassMap<CricketAddition>
    {
        public CricketAdditionMap()
        {
            Id(x => x.CricketID).GeneratedBy.Assigned().Column("cricketid");
            Map(x => x.Atk).Column("ark");
            Map(x => x.Defense).Column("defense");
            Map(x => x.Hp).Column("hp");
            Map(x => x.Mp).Column("mp");
            Map(x => x.MpReply).Column("mpreply");
            Table("cricket_addition");
        }
    }
}
