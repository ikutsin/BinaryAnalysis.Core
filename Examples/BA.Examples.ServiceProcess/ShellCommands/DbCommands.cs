using Autofac;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.ServiceProcess.ShellCommands
{
    public class DbCommands : ShellCommandSet
    {
        protected bool ExecuteOnDb(string executeOnDb)
        {
            try
            {
                return bool.Parse(executeOnDb);
            }
            catch
            {
                return false;
            }
        }

        [CommandDescription("Create database [executeOnDb:bool]")]
        public void Create(string executeOnDb)
        {
            var dbContext = Context.Resolve<IDbContext>();
            var schemaExport = dbContext.GetSchemaExport();
            schemaExport.Create(true, ExecuteOnDb(executeOnDb));
        }

        [CommandDescription("Drop database [executeOnDb:bool]")]
        public void Drop(string executeOnDb)
        {
            var dbContext = Context.Resolve<IDbContext>();
            var schemaExport = dbContext.GetSchemaExport();
            schemaExport.Drop(true, ExecuteOnDb(executeOnDb));
        }
    }
}
