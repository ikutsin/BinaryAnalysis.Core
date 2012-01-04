using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Scheduler;
using BinaryAnalysis.Scheduler.Task.Script;
using BinaryAnalysis.Tests.Helpers.Entities;
using BinaryAnalysis.Tests.Helpers.Scripts;
using BinaryAnalysis.UI.BrowserContext;

namespace BinaryAnalysis.Tests.Helpers
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //BinaryAnalysis.UI
            //builder.RegisterType<DatabaseContextExtension>().As<IBrowserContextExtension>()
            //    .WithMetadata<IBrowserContextExtensionMetadata>(
            //        m => m.For(am => am.Name, "db"));
            builder.RegisterType<LoggingContextExtensions>().As<IBrowserContextExtension>()
                .WithMetadata<IBrowserContextExtensionMetadata>(
                    m => m.For(am => am.Name, "logging"));
            builder.RegisterType<ContextExtensionsHolder>().InstancePerDependency();


            //dependency scripts
            builder.RegisterType<TestDependenciesInContext>()
                .PropertiesAutowired()
                .SingleInstance();

            builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "default", new TaskScriptHandler(
                            (x) => { },
                            typeof(TestDependenciesInContext))}
                    })
                }).Named<TaskParameters>("dep-incontext").InstancePerDependency();

            builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "default", new TaskScriptHandler(
                            (x) => { },
                            typeof(TestDependencies))}
                    })
                }).Named<TaskParameters>("dep-outcontext").InstancePerDependency();

            //empty task parameters
            builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "default", new TaskScriptHandler(
                            (x) => { x.Flow.Schedule("sleep"); })
                        },
                        { "sleep", new SleepIntervalScript() },
                    }), 
                    new NamedParameter("settings", new Dictionary<string, object>
                    {
                        { SleepIntervalScript.SETTING_SLEEP_INTERVAL, 1000 }
                    })
                }).Named<TaskParameters>("sleep-1000").InstancePerDependency();

           builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "sleep", new SleepIntervalScript() }
                    }),
                    new NamedParameter("settings", new Dictionary<string, object>
                    {
                        { SleepIntervalScript.SETTING_SLEEP_INTERVAL, 5000 }
                    })
                }).Named<TaskParameters>("sleep-5000").InstancePerDependency();

           builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "default", new TaskScriptHandler(
                            (x) =>
                                {
                                    x.Flow.FireCustomEvent(x.Settings.Get("setting_message"));
                                })
                        }
                    }), 
                    new NamedParameter("settings", new Dictionary<string, object>
                    {
                        { "setting_message", "fired" }
                    })
                }).Named<TaskParameters>("cusomEvent-fire").InstancePerDependency();

           builder.RegisterType<TaskParameters>()
                .WithParameters(new Parameter[] {
                    new NamedParameter("scripts", new Dictionary<string, ISchedulerTaskScript>
                    {
                        { "default", new TaskScriptHandler(
                            (x) =>
                                {
                                    x.Flow.AddMessage("Did nothing :)");
                                })
                        }
                    })
                }).Named<TaskParameters>("do-nothing").InstancePerDependency();

            //debug listener
            //builder.RegisterType<DebugNHListener>()
            //    .As<ITypedListener>()
            //    .PropertiesAutowired()
            //    .SingleInstance();

            

            //Repositories
            builder.RegisterAssemblyTypes(typeof(TestModule).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsSelf();

            //config
            builder.RegisterType<NHTestConfig>().As<INHMappingsProvider>().SingleInstance();

            base.Load(builder);
        }
    }

    class NHTestConfig : INHMappingsProvider
    {

        public IList<Type> FluentMappings
        {
            get
            {
                return new List<Type> {
                typeof(TestWithSettingEntityMap), 
                typeof(TestEntityMap),
                typeof(IndexableTestEntityMap)
            };}
        }
    }
}
