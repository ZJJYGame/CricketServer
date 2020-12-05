using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class RoleBattleData
    {
        //攻击力
        public int Attack { get; private set; }
        //血量
        public int Health { get; private set; }
        //防御力
        public int Defence { get; private set; }
        //耐力
        public int Endurance { get; private set; }
        //耐力回复
        public int EnduranceReply { get; private set; }
        //行动条
        public int ActionBar { get; private set; }
        //暴击率
        public int CritProp { get; private set; }
        //闪避率
        public int DodgeProp { get; private set; }
        //受到伤害
        public int ReceiveDamage { get; private set; }
        //穿透
        public int Pierce { get; private set; }
        //反伤
        public int ReboundDamage { get; private set; }
        //暴击伤害
        public int CritDamage { get; private set; }
        //暴击抗性
        public int CritResistance { get; private set; }

        public RoleBattleData()
        {
            Attack = 20;
            Health = 100;
            Defence =5;
            Endurance = 200;
            EnduranceReply = 20;
            ActionBar = 1000;
            CritProp = 10;
            DodgeProp = 10;
            ReceiveDamage = 100;
            Pierce = 10;
            ReboundDamage = 10;
            CritDamage = 150;
            CritResistance = 0;
        }
    }
}
