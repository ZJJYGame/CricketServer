﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class CricketMap : ClassMap<Cricket>
    {
        public CricketMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.CricketID).Column("cricketid");
            Map(x => x.CricketName).Column("cricket_name");
            Map(x => x.LevelID).Column("levelid");
            Map(x => x.RankID).Column("rankid");
            Map(x => x.SkillDict).Column("skill_list");
            Map(x => x.HeadPortraitID).Column("head_portraitid");
            Map(x => x.SpecialDict).Column("special_skills");
            Map(x => x.Exp).Column("exp");
            Map(x => x.Roleid).Column("roleid");
            Table("cricket");
        }
    }
}
