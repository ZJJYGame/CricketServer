using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public class BattleBuffController: IRoleBattleData
    {
        #region buff属性
        //攻击力
        public int Attack { get; private set; }
        //最大血量
        public int MaxHealth { get; private set; }
        //血量
        public int Health { get; private set; }
        //防御力
        public int Defence { get; private set; }
        //最大耐力
        public int MaxEndurance { get; private set; }
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
        #endregion

        Dictionary<int, BattleBuffEntity> buffEntityDict = new Dictionary<int, BattleBuffEntity>();

        public IRoleBattleData roleBattleData;

        Action<IRoleBattleData> buffTriggerEvent;
        public event Action<IRoleBattleData> BuffTriggerEvent
        {
            add { buffTriggerEvent += value; }
            remove { buffTriggerEvent -= value; }
        }
        Action<int> buffTimeUpdateEvent;
        public event Action<int> BuffTimeUpdateEvent
        {
            add { buffTimeUpdateEvent += value; }
            remove { buffTimeUpdateEvent -= value; }
        }

        public void ChangeProperty(BattleBuffEffectProperty battleBuffEffectProperty,int changeValue)
        {
            switch (battleBuffEffectProperty)
            {
                case BattleBuffEffectProperty.Attack:
                    Attack += (int)(roleBattleData.Attack * changeValue / 100f);
                    break;
                case BattleBuffEffectProperty.Defense:
                    Defence += (int)(roleBattleData.Defence * changeValue / 100f);
                    break;
                case BattleBuffEffectProperty.Endurance:
                    Endurance += (int)(roleBattleData.Endurance * changeValue / 100f);
                    break;
                case BattleBuffEffectProperty.EnduranceReply:
                    EnduranceReply += (int)(roleBattleData.EnduranceReply * changeValue / 100f);
                    break;
                case BattleBuffEffectProperty.CritProp:
                    CritProp += changeValue;
                    break;
                case BattleBuffEffectProperty.ReduceDamage:
                    ReceiveDamage -= changeValue;
                    break;
                case BattleBuffEffectProperty.Pierce:
                    Pierce += changeValue;
                    break;
                case BattleBuffEffectProperty.ReboundDamage:
                    ReboundDamage += changeValue;
                    break;
                case BattleBuffEffectProperty.CritDamage:
                    CritDamage += changeValue;
                    break;
                case BattleBuffEffectProperty.CritResistance:
                    CritResistance += changeValue;
                    break;
            }
        }
        public void AddBuff(BattleSkillAddBuff battleSkillAddBuff,int skillId)
        {
            int id = Convert.ToInt32(battleSkillAddBuff.BuffId.ToString() + skillId.ToString());
            BattleBuffEntity battleBuffEntity = GameManager.ReferencePoolManager.Spawn<BattleBuffEntity>();
            battleBuffEntity.InitData(battleSkillAddBuff,this,skillId);
            if (buffEntityDict.ContainsKey(id))
            {
                buffEntityDict[id].OnRemove();
                GameManager.ReferencePoolManager.Despawn(buffEntityDict[id]);
                buffEntityDict[id] = battleBuffEntity;
            }
            else
            {
                buffEntityDict.Add(id, battleBuffEntity);
            }
            buffEntityDict[id].OnTrigger(roleBattleData);
        }

        public void RemoveBuff(int id)
        {
            buffEntityDict[id].OnRemove();
            GameManager.ReferencePoolManager.Despawn(buffEntityDict[id]);
            buffEntityDict.Remove(id);
        }

        public void TriggerBuff()
        {
            buffTriggerEvent?.Invoke(roleBattleData);
        }
        public void UpdateBuffTime(int time)
        {
            buffTimeUpdateEvent?.Invoke(time);
        }

        public BattleBuffController(IRoleBattleData roleBattleData)
        {
            this.roleBattleData = roleBattleData;
        }
    }
}
