using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
   public class Cricket
    {
        public virtual int ID { set; get; }
        public virtual int CricketID { set; get; }
        public virtual int LevelID { set; get; }
        public virtual int Exp { set; get; }
        public virtual string CricketName { set; get; }
        public virtual int RankID { set; get; }
        public virtual string SkillDict { set; get; }
        public virtual string SpecialDict { set; get; }
        public virtual int Roleid { set; get; }
        public Cricket()
        {
            ID = -1;
            CricketID =5001;
            LevelID =1;
            Exp = 0;
            RankID = 301;
            CricketName = "蛐蛐1";
            //SkillDict = "{}";
            Dictionary<int, int> dict = new Dictionary<int, int>();
            //dict.Add(3602, 0);
            //dict.Add(3604, 0);
            dict.Add(3606, 0);
            //dict.Add(3613, 0);
            //dict.Add(3615, 0);
            SkillDict = Utility.Json.ToJson(dict);
            SpecialDict = "{}";
            Roleid = -1;//玩家ID
        }
    }
}
