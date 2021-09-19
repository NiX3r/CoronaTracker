using CoronaTracker.Database;
using CoronaTracker.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CoronaTracker.Timers
{
    class RefreshConnectionTimer
    {

        private Timer timer;

        /// <summary>
        /// Constructor for refresh mysql database connection
        /// </summary>
        public RefreshConnectionTimer()
        {
            timer = new Timer(3600000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
        }

        /// <summary>
        /// Function to change status of timer
        /// </summary>
        /// <param name="status"> variable for to change status</param>
        public void ChangeStatus(bool status)
        {
            if (status)
                timer.Start();
            else
                timer.Stop();
        }

        /// <summary>
        /// Function to refresh connection
        /// </summary>
        /// <param name="sender"> variable for sender</param>
        /// <param name="e"> variable for event arguments </param>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            LogClass.Log($"Refreshing database connection");
            DatabaseMethods.RefreshDatabaseConnection();
            LogClass.Log($"Database connection successfully refreshed");
        }
    }
}
