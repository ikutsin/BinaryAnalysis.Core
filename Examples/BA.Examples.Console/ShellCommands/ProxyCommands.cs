using System.Collections.Generic;
using Autofac;
using BinaryAnalysis.Box;
using BinaryAnalysis.Box.Presentations;
using BinaryAnalysis.Extensions.HttpProxy.Data;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
{
    public class ProxyCommands : ShellCommandSet
    {
        string[] lists = new[] { "all", "new", "invalid", "working" };

        [CommandDescription("Count proxies")]
        public string[] Count(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;
            Writer.WriteLine(list.Count);
            return null;
        }

        [CommandDescription("List uris")]
        public string[] Urls(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;

            foreach (HttpProxyEntity httpProxyEntity in list)
            {
                Writer.WriteLine(httpProxyEntity);
            }
            return null;
        }

        [CommandDescription("XML Presentation")]
        public string[] Xml(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;

            var boxed = new Box<HttpProxyBoxMap>(list);
            var presenter = new XmlBoxPresentation();
            Writer.WriteLine(presenter.AsString(boxed));
            return null;
        }

        [CommandDescription("Dumper Presentation")]
        public string[] Dump(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;

            var boxed = new Box<HttpProxyBoxMap>(list);
            var presenter = new DumperBoxPresentation();
            Writer.WriteLine(presenter.AsString(boxed));
            return null;
        }


        [CommandDescription("JSON Presentation")]
        public string[] Json(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;
            var boxed = new Box<HttpProxyBoxMap>(list);
            var presenter = new JsonBoxPresentation();
            Writer.WriteLine(presenter.AsString(boxed));
            return null;
        }

        [CommandDescription("NetDataContract Presentation")]
        public string[] Contract(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;
            var boxed = new Box<HttpProxyBoxMap>(list);
            var presenter = new NetDataContractBoxPresentation();
            Writer.WriteLine(presenter.AsString(boxed));

            return null;
        }

        [CommandDescription("Delete")]
        public string[] Delete(string input)
        {
            IList<HttpProxyEntity> list = GetList(input);
            if (list == null) return lists;

            var repo = Context.Resolve<HttpProxyRepository>();
            repo.DeleteAll(list);
            return null;
        }        

        public void Backup()
        {
            var backup = Context.Resolve<HttpProxyFileBackup>();
            backup.Backup();
        }

        public void Restore()
        {
            var backup = Context.Resolve<HttpProxyFileBackup>();
            backup.Restore();
        }
        
        IList<HttpProxyEntity> GetList(string input)
        {
            if (input == null) return null;
            var repo = Context.Resolve<HttpProxyRepository>();
            switch (input.ToLower())
            {
                case "all": return repo.GetAll();
                case "invalid": return repo.GetInvalid();
                case "new": return repo.GetNewForTesting();
                case "working": return repo.GetWorking();
                default: return null;
            }             
        }
    }
}
