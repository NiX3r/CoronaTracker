using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using CoronaTracker.Utils;
using System;
using System.Windows.Forms;

namespace CoronaTracker
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {

            // Initialize log class
            LogClass.LogClassInitialize();
            // Start initializing program
            ProgramVariables.ProgramThread = new MainProgramThread(args);
            // Run a program
            Application.Run();

        }

    }
}