using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRoleBattleData
    {
        //攻击力
        int Attack { get; }
        //最大血量
        int MaxHealth { get; }
        //血量
        int Health { get; }
        //防御力
        int Defence { get; }
        //最大耐力
        int MaxEndurance { get; }
        //耐力
        int Endurance { get; }
        //耐力回复
        int EnduranceReply { get; }
        //行动条
        int ActionBar { get; }
        //暴击率
        float CritProp { get; }
        //闪避率
        float DodgeProp { get; }
        //受到伤害
        int ReceiveDamage { get; }
        //穿透
        int Pierce { get; }
        //反伤
        int ReboundDamage { get; }
        //暴击伤害
        int CritDamage { get; }
        //暴击抗性
        int CritResistance { get; }
    }
}
