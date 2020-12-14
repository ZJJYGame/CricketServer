using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
   public  class xRTaskMap:ClassMap<xRTask>
    {
        public xRTaskMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.taskDict).Column("taskdict");
            Table("task");
        }
    }
}
