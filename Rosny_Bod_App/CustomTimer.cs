using System;

namespace Rosny_Bod_App
{
    public class CustomTimer
    {
        /// <summary>
        /// Drží čas startu
        /// </summary>
        public DateTime cas { get; set; }

        /// <summary>
        /// Drží aktuální čas
        /// </summary>
        public DateTime cas2 { get; set; }

        /// <summary>
        /// Rozdíl časů
        /// </summary>
        public TimeSpan difference { get; set; }

        /// <summary>
        /// Pracuje časovač?
        /// </summary>
        private bool running { get; set; } = false;

        public void start_timer()
        {
            if (!running)
            {
                cas = DateTime.Now;
            }
            running = true;
        }

        public bool timer_enlapsed(int doba)
        {
            if (!running)
            {
                throw new InvalidOperationException("Timer didnt start");
            }
            cas2 = DateTime.Now;
            difference = cas2 - cas;
            if (difference.TotalSeconds > doba)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void stop_timer()
        {
            cas = DateTime.MinValue;
            cas2 = DateTime.MinValue;
            running = false;
        }
    }
}