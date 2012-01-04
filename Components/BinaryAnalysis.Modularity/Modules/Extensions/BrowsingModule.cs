using System;
using Autofac;
using BinaryAnalysis.Browsing.Windowless;
using BinaryAnalysis.Browsing.Windowless.Decorators;
using BinaryAnalysis.Browsing.Windowless.Proxies;
using BinaryAnalysis.Extensions.Browsing;
using BinaryAnalysis.Extensions.Browsing.Commands;
using BinaryAnalysis.Scheduler.Task.Script;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    public class BrowsingModule : Module
    {
        public int StateStoringDurationSeconds { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            //utility commands
            builder.RegisterType<ClearDomainCacheForScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "ClearDomainCacheFor"));
            builder.RegisterType<GetBrowsingSessionScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "GetBrowsingSession"));
            builder.RegisterType<ClearCacheEntryScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "ClearCacheEntry"));
            builder.RegisterType<SwitchProxyAndMarkInvalidScriptCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "SwitchProxyAndMarkInvalid"));
            builder.RegisterType<SwitchSessionToCommand>()
                .As<IScriptUtilityCommand>()
                .WithMetadata<IScriptUtilityCommandMetadata>(
                    m => m.For(am => am.CommandName, "SwitchSessionTo"));

            //session decorators
            builder.RegisterType<ThrottlingDecorator>()
                .AsSelf().InstancePerDependency()
                .Named<IBrowsingSessionDecorator>("Throttling").InstancePerDependency();
            builder.RegisterType<AutoRefererDecorator>()
                .AsSelf().InstancePerDependency()
                .Named<IBrowsingSessionDecorator>("AutoReferer").InstancePerDependency();
            builder.RegisterType<CommonUserAgentDecorator>()
                .Named<IBrowsingSessionDecorator>("CommonUserAgent").InstancePerDependency();
            builder.RegisterType<SleepRetryDecorator>()
                .AsSelf().InstancePerDependency()
                .Named<IBrowsingSessionDecorator>("SleepRetry").InstancePerDependency();

            //direct browsing session
            builder.RegisterType<BrowsingSession>().InstancePerDependency()
                .Named<IBrowsingSession>("Direct").OnActivating(x =>
                {
                    x.Instance.CurrentProxy = new DirectBrowsing();
                    x.Instance.AddDecorator(x.Context.ResolveNamed<IBrowsingSessionDecorator>("CommonUserAgent"));
                    //x.Instance.AddDecorator(x.Context.ResolveNamed<IBrowsingSessionDecorator>("SleepRetry"));
                });

            //default browsing goal session for StatefullBrowsingSessionWrapper
            builder.RegisterType<StatefullBrowsingSessionWrapper>()
                .InstancePerDependency()
                .OnActivating(
                    x => {
                        x.Instance.Parent = x.Context.ResolveNamed<IBrowsingSession>("Direct");
                        if (StateStoringDurationSeconds > 0)
                            x.Instance.StoringDuration = TimeSpan.FromSeconds(StateStoringDurationSeconds);

                    })
                .As<IBrowsingSession>();

            base.Load(builder);
        }
    }
}
