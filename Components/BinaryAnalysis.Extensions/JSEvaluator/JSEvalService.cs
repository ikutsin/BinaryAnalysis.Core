using System;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using log4net;

namespace BinaryAnalysis.Extensions.JSEvaluator
{
    public class JSEvalService
    {
        private JintEngine context;
        ILog log;
        public JSEvalService(ILog log)
        {
            this.log = log;
            context = new JintEngine();
            //context.SetDebugMode(true);
            context.Step += context_Step;
        }

        public object EvalScriptResult(string script)
        {
            var result = context.Run(script);
            return result;
        }
        public JsObject EvalJsObjectResult(string script)
        {
            return (JsObject)EvalScriptResult(script);
        }
        public JsValue ParseToJsValue(string name, object jsobj)
        {
            JsValue result = new JsValue();
            result.Name = name;
            var obj = (jsobj as JsDictionaryObject);
            if (obj == null) throw new Exception("Unknown js type");

            if (obj.Value != null)
            {
                result.Value = obj.Value.ToString();
            }
            if (obj is JsRegExp)
            {
                result.Value = obj.ToString();
            }


            if (result.Value==null)
            {
                result.Values = new List<JsValue>();
                foreach (var x in (jsobj as JsDictionaryObject))
                {
                    result.Values.Add(ParseToJsValue(x.Key, x.Value));
                }
            }
            return result;

        }

        void context_Step(object sender, Jint.Debugger.DebugInformation e)
        {
            throw new Exception("Comment me if use");
        }
    }
}
