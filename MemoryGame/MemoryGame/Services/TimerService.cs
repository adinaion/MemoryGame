using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Services
{
    public class TimerService
    {
        private int remainingTime;

        public TimerService(int initialTimeSeconds)
        {
            remainingTime = initialTimeSeconds;
        }

        // Decrementează timpul cu 1 secundă și returnează noua valoare
        public int Tick()
        {
            if (remainingTime > 0)
                remainingTime--;
            return remainingTime;
        }

        public bool IsTimeUp()
        {
            return remainingTime <= 0;
        }

        public int GetRemainingTime()
        {
            return remainingTime;
        }
    }
}
