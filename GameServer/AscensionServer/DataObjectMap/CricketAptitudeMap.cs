using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class CricketAptitudeMap : ClassMap<CricketAptitude>
    {
        public CricketAptitudeMap()
        {
            Id(x => x.CricketID).GeneratedBy.Assigned().Column("cricketid");
            Map(x => x.Str).Column("str");
            Map(x => x.Con).Column("con");
            Map(x => x.Dex).Column("dex");
            Map(x => x.Def).Column("def");
            Map(x => x.StrAptitude).Column("str_aptitude");
            Map(x => x.ConAptitude).Column("con_aptitude");
            Map(x => x.DexAptitude).Column("dex_aptitude");
            Map(x => x.DefAptitude).Column("def_aptitude");
            Table("cricket_aptitude");
        }
    }
}
