using Autofac;
using BA.Examples.Console.ShellCommands;
using BinaryAnalysis.Modularity;
using BinaryAnalysis.Scheduler.Scheduler;
using BinaryAnalysis.Terminal;
using log4net;

namespace BA.Examples.Console
{
    //-a="task run Fingerprint-test"
    //-a="task run InfoQuery-test"
    //-a="backup restore infoQueries"
    //-a="backup restore fingerprints"
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static bool IsRestartPending { get; set; }
        public static Shell Shell { get; private set; }
        public static Bootstrap Bootstrap { get; private set; }
        public static SchedulerInstance ScheduleInstance { get; private set; }

        public static void DisposeComponents()
        {
            ScheduleInstance.Stop();
            log.Debug("Components disposed");
        }
        public static void InitComponents()
        {
            ScheduleInstance = Bootstrap.Container.Resolve<SchedulerInstance>();
            ScheduleInstance.Start(Bootstrap.Container);
            log.Debug("Components initialized");
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Bootstrap = new Bootstrap();
            Shell = new Shell();
            var unprocessed = Shell.ProcessArguments(args, Bootstrap.Container, typeof(TopLevelCommands));

            if (unprocessed.Length > 0)
            {
                //var noServiceFlag = unprocessed.Where(x => x.Contains("no"));
                //if (noServiceFlag.Count()>0)
                //{
                //    unprocessed = unprocessed.Where(x => !noServiceFlag.Contains(x)).ToArray();
                //}
                if (unprocessed.Length > 0)
                {
                    log.Warn("Unprocessed arguments: " + string.Join(", ", unprocessed));
                }
            }

            //Bootstrap.Container.Resolve<PerformanceCountersTracker>().Start(
            //    TimeSpan.FromSeconds(10), "Performance");

            //simple console wait));)
            if (args.Length == 0)
            {
                InitComponents();
                ProcessLock.LockByKey();
                DisposeComponents();
            }
            else if (Shell.IsShellEnabled)
            {
                Shell.StartLineEditor();
            }
            Bootstrap.Dispose();

            //ProcessLock.LockByLine();
        }
    }
}
