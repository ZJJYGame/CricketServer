using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class InventoryMap: ClassMap<Inventory>
    {
        public InventoryMap()
        {
            Id(x => x.RoleId).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.ItemDict).Column("itemdict");
            Table("inventory");
        }
    }
}
