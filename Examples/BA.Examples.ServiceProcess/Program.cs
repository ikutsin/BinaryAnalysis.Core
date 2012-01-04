using Autofac;
using BA.Examples.ServiceProcess.ShellCommands;
using BinaryAnalysis.Modularity;
using BinaryAnalysis.Scheduler.Scheduler;
using BinaryAnalysis.Terminal;
using log4net;

namespace BA.Examples.ServiceProcess
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public static bool IsRestartPending { get; set; }
        public static Shell Shell { get; private set; }
        public static Bootstrap Bootstrap { get; private set; }
        public static SchedulerInstance ScheduleInstance { get; private set; }
        public static ServiceBinding ServiceBinding { get; private set; }

        public static void DisposeComponents()
        {
            ServiceBinding.Stop();
            ScheduleInstance.Stop();
            log.Debug("Components disposed");
        }
        public static void InitComponents()
        {
            ScheduleInstance = Bootstrap.Container.Resolve<SchedulerInstance>();
            ScheduleInstance.Start(Bootstrap.Container);

            ServiceBinding = new ServiceBinding(Bootstrap.Container);
            ServiceBinding.Start();

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
                log.Warn("Unprocessed arguments: "+string.Join(", ", unprocessed));
            }

            //simple console wait
            if (args.Length == 0)
            {
                InitComponents();
                ProcessLock.LockByKey();
                DisposeComponents();
            }
            else if(Shell.IsShellEnabled)
            {
                InitComponents();
                Shell.StartLineEditor();
                DisposeComponents();
            }
            Bootstrap.Dispose();
        }
    }
}
