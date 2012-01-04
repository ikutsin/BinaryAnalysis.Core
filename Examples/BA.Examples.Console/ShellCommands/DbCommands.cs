using Autofac;
using BA.Examples.Console.Data;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
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
        [CommandDescription("Test HQL")]
        public void HQL(string query)
        {
            var action = Context.Resolve<HQLQueryAction>();
            action.Query = query;
            var result = action.ActionCommand();
            var presenter = new DumperBoxPresentation();
            var box = BoxExtensions.CreateFor(typeof(object));
            foreach (var o in result)
            {
                box.Add(o);
            }
            Writer.WriteLine(presenter.AsString(box));
        }
    }
}
