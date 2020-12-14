using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    public static partial class RoleCricketManager
    {

        public enum PropType
        {
            AddExp = 1,
            AddStr = 2,
            AddCon = 3,
            AddDex = 4,
            AddDef = 5,
            AddAtk = 6,
            AddDefense = 7,
            AddHp = 8,
            AddMp = 9,
            AddMpReply = 10,
            Skill = 11,
            DeleteSkill = 12,
            Reset=13,
        }

        public static void DifferentiateGlobal(int propid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDataDict);

            var result = propDataDict.TryGetValue(propid, out var propData);
            if (result)
            {
                switch ((PropType)propData.PropType)
                {
                    case PropType.AddExp:
                        break;
                    case PropType.AddStr:
                        break;
                    case PropType.AddCon:
                        break;
                    case PropType.AddDef:
                        break;
                    case PropType.AddDex:
                        break;
                    case PropType.DeleteSkill:
                        break;
                    case PropType.AddAtk:
                        break;
                    case PropType.AddDefense:
                        break;
                    case PropType.AddHp:
                        break;
                    case PropType.AddMp:
                        break;
                    case PropType.AddMpReply:
                        break;
                    case PropType.Skill:
                        break;
                    case PropType.Reset:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 区分加成物品
        /// </summary>
        public static CricketAptitude AptitudeProp(PropData propData,CricketAptitude cricketAptitude)
        {
            switch ((PropType)propData.PropType)
            {
                case PropType.AddStr:
                    cricketAptitude.Str += propData.AddNumber;
                    break;
                case PropType.AddCon:
                    cricketAptitude.Con += propData.AddNumber;
                    break;
                case PropType.AddDef:
                    cricketAptitude.Def += propData.AddNumber;
                    break;
                case PropType.AddDex:
                    cricketAptitude.Dex += propData.AddNumber;
                    break;
                default:
                    break;
            }

            return cricketAptitude;
        }
        /// <summary>
        /// 区分属性加成
        /// </summary>
        /// <param name="propData"></param>
        /// <param name="cricketStatus"></param>
        public static CricketStatus StatusProp(PropData propData, CricketStatus cricketStatus)
        {
            switch ((PropType)propData.PropType)
            {
                case PropType.AddAtk:
                    cricketStatus.Atk += propData.AddNumber;
                    break;
                case PropType.AddDefense:
                    cricketStatus.Defense += propData.AddNumber;
                    break;
                case PropType.AddHp:
                    cricketStatus.Hp += propData.AddNumber;
                    break;
                case PropType.AddMp:
                    cricketStatus.Mp += propData.AddNumber;
                    break;
                case PropType.AddMpReply:
                    cricketStatus.MpReply += propData.AddNumber;
                    break;
                default:
                    break;
            }
            return cricketStatus;
        }
        /// <summary>
        /// 区分消耗物品
        /// </summary>
        public static void ConsumeProp(PropData propData)
        {
            switch ((PropType)propData.PropType)
            {
                case PropType.AddExp:

                    break;         
                case PropType.DeleteSkill:
                    break;              
                case PropType.Skill:
                    break;
                case PropType.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
