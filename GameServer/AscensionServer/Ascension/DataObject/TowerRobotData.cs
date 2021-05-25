using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class TowerRobotData
    {
        public int RankID; // 段位id
        public int CricketId;
        public string UserName; // 机器人名字
        public int Level; // 蛐蛐等级
        public string CricketName; // 蛐蛐名称
        public string CricketIcon; // 蛐蛐图片
        public string CricketModel; // 蛐蛐模型
        public int Atk; // 攻击力
        public int Hp; // 生命值
        public int Defense; // 防御力
        public int Mp; // 耐力
        public int MpReply; // 耐力回复
        public float Crt; // 暴击率
        public float Eva; // 闪避
        public int Speed; // 行动条
        public List<int> SkillPool; // 技能池
        public int CrtAtk; // 暴击伤害
        public int CrtDef; // 暴击抗性
        public int ReduceAtk; // 减伤
        public int ReduceDef; // 穿透
        public int Rebound; // 反伤
    }
}
