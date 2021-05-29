using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CoronaTracker.Timers
{
    class RefreshCovidDataTimer
    {

        private static Timer timer = new Timer(3600000);

        public static void SetupTimer()
        {
            timer.Elapsed += on_timer;
            timer.Start();
        }

        private static void on_timer(object sender, ElapsedEventArgs e)
        {

            RestAPI.GetCovidDataAsync();

        }
    }
}
