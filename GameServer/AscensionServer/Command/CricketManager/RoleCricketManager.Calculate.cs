﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer
{
    public static partial class RoleCricketManager
    {
        /// <summary>
        /// 重置蟋蟀加点
        /// </summary>
        /// <param name="level">当前蟋蟀等级</param>
        /// <returns></returns>
        public static void ReSetPoint(int roleid, int cricketid, int propid)
        {
            int points = 0;
            var nHCriteria = xRCommon.xRNHCriteria("CricketID", cricketid);
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteria);
            var aptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteria);
            var addition = xRCommon.xRCriteria<CricketAddition>(nHCriteria);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);
            if (addition != null && aptitude != null && point != null && cricket != null)
            {
                for (int i = 1; i <= cricket.LevelID; i++)
                {
                    points += cricketLevelDict[i].AssignPoint;
                }
                point.FreePoint = points;
                point.Str = 0;
                point.Dex = 0;
                point.Def = 0;
                point.Con = 0;
                NHibernateQuerier.Update(point);
                var status = CalculateStutas(aptitude, point, addition);
                var data = xRCommon.xRS2CParams();
                data.Add((byte)ParameterCode.CricketAptitude, aptitude);
                data.Add((byte)ParameterCode.CricketStatus, status);
                data.Add((byte)ParameterCode.CricketPoint, point);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)CricketOperateType.AddPoint, Utility.Json.ToJson(data));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Success, dict);

                InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { propid, new ItemDTO() { ItemAmount = 1 } } });
            } else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);



        }
        /// <summary>
        /// 计算属性加成
        /// </summary>
        public static CricketStatus CalculateStutas(CricketAptitude cricketAptitude, CricketPoint cricketPoint, CricketAddition cricketAddition)
        {
            //TODO补充技能的加成
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketStatusData>>(out var StatusDict);
            CricketStatus cricketStatus = new CricketStatus();

            cricketStatus.Atk = cricketAddition.Atk + StatusDict[1].Atk + (int)((cricketAptitude.Str + cricketPoint.Str) * (cricketAptitude.StrAptitude + 100) * 0.01f);
            cricketStatus.Defense = cricketAddition.Defense + StatusDict[1].Defense + (int)((cricketAptitude.Def + cricketPoint.Def) * (cricketAptitude.DefAptitude + 100) * 0.005f);
            cricketStatus.Hp = cricketAddition.Hp + StatusDict[1].Hp + (int)((cricketAptitude.Con + cricketPoint.Con) * (cricketAptitude.ConAptitude + 100) * 0.05f);
            cricketStatus.Mp = cricketAddition.Mp + StatusDict[1].Mp + (int)(cricketStatus.Hp / 100) + (cricketStatus.Mp);
            cricketStatus.MpReply = cricketAddition.MpReply + StatusDict[1].MpReply + (int)(cricketStatus.Mp / 10) + cricketStatus.MpReply;
            cricketStatus.Crt = (cricketAptitude.Dex + cricketPoint.Dex) * (300 - (2 * (100 - cricketAptitude.DexAptitude))) / 1000000f;
            cricketStatus.Eva = (cricketAptitude.Dex + cricketPoint.Dex) * (300 - (2 * (100 - cricketAptitude.DexAptitude))) / 1000000f;
            cricketStatus.Speed = StatusDict[1].Speed -  ((cricketAptitude.Dex + cricketPoint .Dex) * (1.5f - (0.01 * (100 - cricketAptitude.DefAptitude))));
            cricketStatus.ReduceAtk = StatusDict[1].ReduceAtk;
            cricketStatus.ReduceDef = StatusDict[1].ReduceDef;
            cricketStatus.Rebound = StatusDict[1].Rebound;
            Utility.Debug.LogInfo("闪避固定值" + (cricketAptitude.Dex + cricketPoint.Dex) +"资质"+ cricketAptitude.DexAptitude+"值"+ (300 - (2 * (100f - cricketAptitude.DexAptitude))) / 1000000f+"计算值"+ cricketStatus.Eva);
            //Utility.Debug.LogInfo("攻击" +(cricketAddition.Atk + StatusDict[1].Atk) + "防御" + (cricketAptitude.Str + cricketPoint.Str) + "血量" + (cricketAptitude.StrAptitude + 100) * 0.01f);
            return cricketStatus;
        }
        /// <summary>
        /// 获取资质等随机随机
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="num"></param>
        public static int RandomNum(int min, int max)
        {
            int num = 50;
            Random random = new Random();
            num = random.Next(min, max);
            return num;
        }
        /// <summary>
        /// 加经验升级
        /// </summary>
        public static void UpdateLevel(int cricketid, PropData propData, int roleid)
        {
            var nHCriteria = xRCommon.xRNHCriteria("CricketID", cricketid);
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteria);
            if (cricket != null || point != null)
            {
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);
                cricket.Exp += propData.AddNumber;
                var tempcricket = new Cricket() { Exp = cricket.Exp, LevelID = cricket.LevelID };
                tempcricket = LevelUpCalculate(tempcricket);
                for (int i = cricket.LevelID + 1; i <= tempcricket.LevelID; i++)
                {
                    point.FreePoint += cricketLevelDict[i].AssignPoint;
                }
                cricket.Exp = tempcricket.Exp;
                cricket.LevelID = tempcricket.LevelID;
                var data = xRCommon.xRS2CParams();
                data.Add((byte)ParameterCode.Cricket, SetCricketValue(cricket));
                data.Add((byte)ParameterCode.CricketPoint, point);
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)CricketOperateType.LevelUp, Utility.Json.ToJson(data));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, dict);
                NHibernateQuerier.Update(cricket);
                NHibernateQuerier.Update(point);
                if (propData.PropID != -1)
                {
                    InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { propData.PropID, new ItemDTO() { ItemAmount = 1 } } });
                }
            }
            else
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
            }
        }
        /// <summary>
        /// 学习技能
        /// </summary>
        public static void StudySkill(int prop, int cricketid, int roleid)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PropData>>(out var propDict);
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);
            if (cricket != null)
            {
                if (propDict.ContainsKey(prop))
                {
                    #region 
                    var skills = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict);
                    if (skills.ContainsKey(propDict[prop].SkillID))
                    {
                        xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                        return;
                    }
                    else
                    {
                        skills.Add(propDict[prop].SkillID, 0);
                        //返回成功并更新数据库
                        cricket.SkillDict = Utility.Json.ToJson(skills);
                        NHibernateQuerier.Update(cricket);
                        var data = xRCommon.xRS2CParams();
                        data.Add((byte)ParameterCode.Cricket, SetCricketValue(cricket));
                        data.Add((byte)ParameterCode.CricketStatus, cricketstatus);
                        var dict = xRCommon.xRS2CSub();
                        dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                        xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Success, dict);

                        InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { prop, new ItemDTO() { ItemAmount = 1 } } });
                        return;
                    }
                    #endregion
                }
                else
                {
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    return;
                }
            }
            else
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                return;//返回失败
            }

        }
        /// <summary>
        /// 删除技能
        /// </summary>
        public static void RemoveSkill(int prop, int cricketid, int roleid)
        {
            var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
            var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
            var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);

            if (cricket != null)
            {
                var skillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict);

                if (skillDict.Count > 0)
                {
                    Random random = new Random();
                    var num = random.Next(0, skillDict.Count);
                    var skilllist = skillDict.Keys.ToList<int>();
                    skillDict.Remove(skilllist[num]);
                    Utility.Debug.LogInfo("YZQ删除技能后的蛐蛐" + skillDict.Count + "下标" + num);
                    cricket.SkillDict = Utility.Json.ToJson(skillDict);
                    NHibernateQuerier.Update(cricket);
                    var data = xRCommon.xRS2CParams();
                    data.Add((byte)ParameterCode.Cricket, SetCricketValue(cricket));
                    data.Add((byte)ParameterCode.CricketStatus, cricketstatus);
                    var dict = xRCommon.xRS2CSub();
                    dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Success, dict);
                    InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { prop, new ItemDTO() { ItemAmount = 1 } } });
                    return;
                }
                else
                {
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    return;
                }
            }
            else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Fail, xRCommonTip.xR_err_Verify);


        }
        /// <summary>
        /// 添加特殊技能
        /// </summary>
        /// <param name="skillid"></param>
        /// <param name="roleid"></param>
        public static void AddSpecialSkill(int skillid, int level, int roleid, int cricketid)
        {
            Utility.Json.ToJson("添加的特殊技能为" + skillid+"蛐蛐id为"+ cricketid);
            var nHCriteria = xRCommon.xRNHCriteria("ID", cricketid);
            var nHCriteriacricket = xRCommon.xRNHCriteria("CricketID", cricketid);
            var cricket = xRCommon.xRCriteria<Cricket>(nHCriteria);
            var point = xRCommon.xRCriteria<CricketPoint>(nHCriteriacricket);
            var aptitude = xRCommon.xRCriteria<CricketAptitude>(nHCriteriacricket);
            var addition = xRCommon.xRCriteria<CricketAddition>(nHCriteriacricket);
            var skills = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict);
            if (cricket != null && point != null && aptitude != null && addition != null)
            {
                if (!skills.ContainsKey(skillid))
                {
                    skills.Add(skillid, level);
                }
                else
                {
                    if (skills[skillid] < 10)
                    {
                        if (skills[skillid] + level < 10)
                        {
                            skills[skillid] += level;
                        }
                        else
                            skills[skillid] = 10;

                    }
                }

                cricket.SpecialDict = Utility.Json.ToJson(skills);
                Utility.Json.ToJson("添加的特殊技能为"+ skillid);
                var status = CalculateStutas(aptitude, point, addition);
                status.CricketID = cricket.ID;
                NHibernateQuerier.Update(cricket);
                NHibernateQuerier.Update(status);

                var data = xRCommon.xRS2CParams();
                data.Add((byte)ParameterCode.CricketStatus, status);
                data.Add((byte)ParameterCode.Cricket, SetCricketValue(cricket));
                var dict = xRCommon.xRS2CSub();
                dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Success, dict);
            }
            else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);

        }
        /// <summary>
        /// 计算加经验的
        /// </summary>
        /// <param name="cricket"></param>
        /// <returns></returns>
        public static Cricket LevelUpCalculate(Cricket cricket)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, CricketLevel>>(out var cricketLevelDict);
            if (cricket.Exp >= cricketLevelDict[cricket.LevelID].ExpUP)
            {
                cricket.Exp -= cricketLevelDict[cricket.LevelID].ExpUP;
                cricket.LevelID += 1;
                LevelUpCalculate(cricket);
            }
            return cricket;
        }
        /// <summary>
        /// 设置cricketDTO字段赋值
        /// </summary>
        /// <param name="cricket"></param>
        /// <returns></returns>
        public static CricketDTO SetCricketValue(Cricket cricket)
        {
            CricketDTO cricketDTO = new CricketDTO();

            cricketDTO.ID = cricket.ID;
            cricketDTO.CricketID = cricket.CricketID;
            cricketDTO.CricketName = cricket.CricketName;
            cricketDTO.Exp = cricket.Exp;
            cricketDTO.LevelID = cricket.LevelID;
            cricketDTO.RankID = cricket.RankID;
            cricketDTO.SkillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict);
            cricketDTO.SpecialDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SpecialDict);

            return cricketDTO;
        }
        /// <summary>
        /// 升级蛐蛐技能
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="propData"></param>
        /// <param name="cricket"></param>
        public static void UpdateCricketSkill(int roleid, RolepPropDTO rolepPropDTO, int cricketid)
        {
            var result = VerifyProp(rolepPropDTO.PropID, PropType.Skill, out var prop);
            if (!InventoryManager.xRVerifyInventory(roleid, rolepPropDTO.PropID, rolepPropDTO.PropNum))
            {
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_VerifyProp);
                return;
            }

            if (result)
            {
                var nHCriteriacricket = xRCommon.xRNHCriteria("ID", cricketid);
                var nHCriteriastatus = xRCommon.xRNHCriteria("CricketID", cricketid);
                var cricket = xRCommon.xRCriteria<Cricket>(nHCriteriacricket);
                var cricketstatus = xRCommon.xRCriteria<CricketStatus>(nHCriteriastatus);

                if (cricket != null && cricketstatus != null)
                {
                    var skillDict = Utility.Json.ToObject<Dictionary<int, int>>(cricket.SkillDict);
                    var exits = skillDict.TryGetValue(prop.SkillID, out int level);
                    Utility.Debug.LogInfo("YZQ升级技能" + exits + level);
                    if (exits)
                    {
                        if (level < 10 && (level + 1) == rolepPropDTO.PropNum)
                        {
                            level += 1;
                            skillDict[prop.SkillID] = level;

                            InventoryManager.xRUpdateInventory(roleid, new Dictionary<int, ItemDTO> { { rolepPropDTO.PropID, new ItemDTO() { ItemAmount = rolepPropDTO.PropNum } } });

                            cricket.SkillDict = Utility.Json.ToJson(skillDict);
                            NHibernateQuerier.Update(cricket);
                            var data = xRCommon.xRS2CParams();
                            data.Add((byte)ParameterCode.Cricket, SetCricketValue(cricket));
                            data.Add((byte)ParameterCode.CricketStatus, cricketstatus);
                            var dict = xRCommon.xRS2CSub();
                            dict.Add((byte)CricketOperateType.UpdateSkill, Utility.Json.ToJson(data));
                            xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (byte)ReturnCode.Success, dict);

                        }
                        else
                            xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
                    }
                } else
                    xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
            } else
                xRCommon.xRS2CSend(roleid, (ushort)ATCmd.SyncCricket, (short)ReturnCode.Fail, xRCommonTip.xR_err_Verify);
        }

        public static void SkillAdditionStatus(Dictionary<int,int> skills, Cricket  cricket,CricketStatus cricketStatus, CricketAptitude cricketAptitude, CricketPoint cricketPoint, CricketAddition cricketAddition)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PassiveSkill>>(out var passiveSkill);

            var  pointFixed = new CricketPoint();
            var statusPercentage = new CricketStatus();
            var statusFixed = new CricketStatus();
            Dictionary<int, int> pairs = new Dictionary<int, int>();
    

            foreach (var item in skills)
            {
                var result = passiveSkill.TryGetValue(item.Key, out var passive);
                if (result)
                {
                    for (int i = 0; i < passive.Attribute.Count; i++)
                    {
                        switch ((SkillAdditionType)passive.Attribute[i])
                        {
                            case SkillAdditionType.Str:
                                pointFixed.Str += passive.Fixed[i] + passive.LevelFixed[i]* cricket.LevelID;
                                break;
                            case SkillAdditionType.Con:
                                pointFixed.Con += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Dex:
                                pointFixed.Dex += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Def:
                                pointFixed.Def += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Atk:
                                cricketStatus.Atk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Atk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Atk+= passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Hp:
                                cricketStatus.Hp += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Hp += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Hp += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Defense:
                                cricketStatus.Defense += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Defense += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Defense += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Mp:
                                cricketStatus.Mp += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Mp += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Mp += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.MpReply:
                                cricketStatus.MpReply += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.MpReply += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.MpReply += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Speed:
                                cricketStatus.Speed += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Speed += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Speed += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Crt:
                                cricketStatus.Crt += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Crt += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Crt += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Eva:
                                cricketStatus.Eva += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Eva += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Eva += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.ReduceAtk:
                                cricketStatus.ReduceAtk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.ReduceAtk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.ReduceAtk += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.ReduceDef:
                                cricketStatus.ReduceDef += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.ReduceDef += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.ReduceDef += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.Rebound:
                                cricketStatus.Rebound += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.Rebound += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.Rebound += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.CrtAtk:
                                cricketStatus.CrtAtk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.CrtAtk += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.CrtAtk += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            case SkillAdditionType.CrtDef:
                                cricketStatus.CrtDef += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusFixed.CrtDef += passive.Fixed[i] + passive.LevelFixed[i] * cricket.LevelID;
                                statusPercentage.CrtDef += passive.Percentage[i] + passive.LevelPercentage[i] * cricket.LevelID;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }



        }
        public enum SkillAdditionType
        {
            Str=1,
            Con=2,
            Dex =3,
            Def=4,
            Atk=5,
            Hp=6,
            Defense=7,
            Mp=8,
            MpReply=9,
            Speed=10,
            Crt=11,
            Eva=12,
            ReduceAtk=13,
            ReduceDef=14,
            Rebound=15,
            CrtAtk=16,
            CrtDef=17
        }
    }
}
