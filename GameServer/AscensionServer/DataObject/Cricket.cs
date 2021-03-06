﻿using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionServer
{
   public class Cricket:IComparable
    {
        public virtual int ID { set; get; }
        public virtual int CricketID { set; get; }
        public virtual int LevelID { set; get; }
        public virtual int Exp { set; get; }
        public virtual string CricketName { set; get; }
        public virtual int RankID { set; get; }
        public virtual string SkillDict { set; get; }
        public virtual string SpecialDict { set; get; }
        public virtual int HeadPortraitID { set; get; }
        public virtual int Roleid { set; get; }
        public Cricket()
        {
            ID = -1;
            CricketID =5001;
            LevelID =1;
            Exp = 0;
            RankID = 301;
            CricketName = "蛐蛐1";
            SkillDict = "{}";
            //Dictionary<int, int> Dict = new Dictionary<int, int>();
            //Dict.Add(3611,0);
            //SkillDict = Utility.Json.ToJson(Dict); 
            SpecialDict = "{}";
            //Dictionary<int, int> Dict = new Dictionary<int, int>();
            //Dict.Add(3339, 0);
            //Dict.Add(3613, 0);
            //SpecialDict = Utility.Json.ToJson(Dict);
            Roleid = -1;//玩家ID
        }



        public virtual int CompareTo(object other)
        {
            Cricket cricket = other as Cricket;
            if (RankID == cricket.RankID)
                return 0;
            else if (RankID > cricket.RankID)
                return -1;
            else
                return 1;
        }
    }
}
