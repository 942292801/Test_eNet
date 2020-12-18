using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNet编辑器
{
    class TimeOutHelper
    {
        private readonly int TimeoutInterval = 2;//超时时间
        public long lastTicks;//用于存储新建操作开始的时间
        public long elapsedTicks;//用于存储操作消耗的时间
        public TimeOutHelper()
        {
            lastTicks = DateTime.Now.Ticks;
        }
        public bool IsTimeout()
        {
            elapsedTicks = DateTime.Now.Ticks - lastTicks;
            TimeSpan span = new TimeSpan(elapsedTicks);
            double diff = span.TotalSeconds;
            if (diff > TimeoutInterval)
                return true;
            else
                return false;
        }

    }
}
