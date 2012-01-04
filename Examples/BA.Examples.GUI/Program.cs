using System;
using BinaryAnalysis.UI.Commons;

namespace BA.Examples.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ProgramContext.StartApplication<MainForm>();
            //ProgramContext.StartApplication<TextViewerWindow>(false);
        }
    }
}
