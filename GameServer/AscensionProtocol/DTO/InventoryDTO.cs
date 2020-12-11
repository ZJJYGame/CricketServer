using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    [Serializable]
    public class InventoryDTO
    {
        public int RoleID { get; set; }
        public Dictionary<int, ItemDTO> ItemDTO { get; set; }
    }

    [Serializable]
    public class ItemDTO
    {
        //public int ItemId { get; set; }
        public int ItemAmount { get; set; }
    }
}
