using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AscensionServer
{


    /// <summary>
    /// 倒计时开始针对的是进入战斗
    /// </summary>
    public delegate void BattleStartDelegateHandle();

    public class TimerManager
    {
        #region 开启倒计时

        public BattleStartDelegateHandle startDelegateHandle;

        System.Timers.Timer Mytimer;
        public TimerManager(int second)
        {
            Mytimer = new System.Timers.Timer(second);
        }


        public void BattleStartTimer()
        {
            Mytimer.Enabled = true;
            Mytimer.Start();
            Mytimer.Elapsed += new ElapsedEventHandler(StartBattleMethodCallBack);
            Mytimer.AutoReset = false;
        }

        public void StartBattleMethodCallBack(object sender, ElapsedEventArgs args)
        {
            startDelegateHandle += BattleStartCallBackMethod;
            startDelegateHandle?.Invoke();
        }

        /// <summary>
        /// 倒计时回调
        /// </summary>
        private void BattleStartCallBackMethod()
        {

        }

        public void BattleStartStopTimer()
        {
            startDelegateHandle -= BattleStartCallBackMethod;
            Mytimer.Stop();
        }
        #endregion
    }
}
