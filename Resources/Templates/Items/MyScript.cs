using System;
using System.Collections.Generic;
using BinaryAnalysis.Browsing.Extensions;
using BinaryAnalysis.Scheduler.Task.Script;

namespace $rootnamespace$
{
    public class $safeitemname$Dependencies : SchedulerTaskScriptDependencies
    {
        public override IEnumerable<string> RequiredSettings
        {
            get { return new[] { $safeitemname$.SETTING_EXAMPLE }; }
        }
    }
    public class $safeitemname$ : AbstractTaskScript
    {
        public const string SETTING_EXAMPLE = "$safeitemname$_example";
        public const string SETTING_RESULT = "$safeitemname$_result";


        public override void Execute()
        {
			//init
			$safeitemname$Dependencies deps = Dependencies as $safeitemname$Dependencies;
			string input = x.Settings.Get<string>(SETTING_EXAMPLE);
			x.Flow.IsNull(input).Fail("Invalid input parameters");
            
			//logic
			string addr = String.Format("http://sameip.org/ip/{0}","");
            var response = x.BrowserWL.NavigateGet(new Uri(addr));
            var doc = response.AsHtmlDocument();
			
			//result
            x.Settings.Set(SETTING_RESULT, result);
        }

        public override Type DependencyClassType
        {
            get { return typeof($safeitemname$Dependencies); }
        }
    }
}
