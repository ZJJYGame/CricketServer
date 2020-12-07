using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;


namespace AscensionServer
{
    /// <summary>
    /// 服务器数据转换
    /// </summary>
    [ImplementProvider]
    public class ServerDataConvertor : IDataConvertor
    {
        public void ConvertData()
        {
            try
            {

                #region 获取Json文件转换成对应数据类
                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(BattleAttackSkillData).Name, out var battleAttackSkillDataData);
                var battleAttackSkillDataDict = TransObject<List<BattleAttackSkillData>>(battleAttackSkillDataData).ToDictionary(key => key.skillId, value => value);
                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(CricketStatus).Name, out var CricketStatusData);
                var CricketStatusDict = TransObject<List<CricketStatus>>(CricketStatusData).ToDictionary(key => key.Level, value => value);
                #endregion

                #region 储存方式
                GameManager.CustomeModule<DataManager>().TryAdd(battleAttackSkillDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(CricketStatusDict);
                #endregion

                #region 获取方式
                //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var set);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + skillGongFaDict[21001].Skill_Describe);
                #endregion
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }

        /// <summary>
        /// 转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T TransObject<T>(string data)
        {
            return Utility.Json.ToObject<T>(data);
        }
    }
}
