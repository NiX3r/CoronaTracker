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

            LogClass.LogClassInitialize();
            ProgramVariables.ProgramThread = new MainProgramThread(args);
            Application.Run();

        }

    }
}