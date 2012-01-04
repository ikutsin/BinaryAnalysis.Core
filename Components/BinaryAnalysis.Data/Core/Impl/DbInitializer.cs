using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Autofac;
using BinaryAnalysis.Data.Index;
using log4net;

namespace BinaryAnalysis.Data.Core.Impl
{
    public enum DatabaseInitMethod : byte
    {
        DoNothing = 0, CreateIfInvalid, Recreate, FailIfInvalid
    }
    public static class DbInitializer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DbInitializer));

        public static void Init(DatabaseInitMethod structureBootstrap, 
            IDbContext factory, IComponentContext ctx)
        {
            IndexRepository indexRepo = null;
            if (ctx.IsRegistered<IndexRepository>())
            {
                //cant take from context because IDbContext is initializing
                indexRepo = new IndexRepository(factory, log);
            }

            log.Info(String.Format("Initializing DB {0} {1} index module",
                structureBootstrap, (indexRepo==null?"without":"with")));

            switch (structureBootstrap)
            {
                case DatabaseInitMethod.DoNothing:
                    if(indexRepo!=null)
                    {
                        indexRepo.IndexManagers.ForEach(x => x.Optimize());
                    }
                    break;
                case DatabaseInitMethod.Recreate:
                    var schema = factory.GetSchemaExport();
                    schema.Create(true, true);

                    if (indexRepo != null)
                    {
                        indexRepo.IndexManagers.ForEach(x => x.Fill());
                    }
                   
                    break;
                case DatabaseInitMethod.FailIfInvalid:
                    try
                    {
                        factory.ValidateSchema();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("FailIfInvalid valiadtion failed", ex);
                    }
                    Init(DatabaseInitMethod.DoNothing, factory, ctx);
                    break;
                case DatabaseInitMethod.CreateIfInvalid:
                    var newstructureBootstrapMethod = DatabaseInitMethod.DoNothing;
                    //Test connection
                    try
                    {
                        factory.ValidateSchema();
                        if (indexRepo != null)
                        {
                            indexRepo.IndexManagers.ForEach(x => x.Purge());
                            indexRepo.IndexManagers.ForEach(x => x.Fill());
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Debug("CreateIfInvalid valiadtion failed");
                        log.Debug(ex);
                        newstructureBootstrapMethod = DatabaseInitMethod.Recreate;
                    }
                    Init(newstructureBootstrapMethod, factory, ctx);

                    break;
            }
        }
    }
}
