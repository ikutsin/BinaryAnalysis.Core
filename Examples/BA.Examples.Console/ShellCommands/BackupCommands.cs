using System;
using System.IO;
using System.Linq;
using Autofac;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Terminal.Commanding;

namespace BA.Examples.Console.ShellCommands
{
    public class BackupCommands : ShellCommandSet
    {
        [CommandDescription("Deleta all from table by name")]
        public string[] Count(string name)
        {
            if (name == null)
            {
                return ConsoleConfigSection.GetConfig().Clean
                    .Cast<NameValueConfigElement>()
                    .Select(x => x.Name).ToArray();
            }
            var stringType = ConsoleConfigSection.GetConfig().Clean
                .Cast<NameValueConfigElement>().First(x => x.Name == name).Value;

            var type = Type.GetType(stringType);
            if (type == null) throw new Exception(stringType + " can not be resolved.");
            
            var repoFinder = Context.Resolve<RepositoryFinder>();
            var repo = repoFinder.CreateRepository(type);

            var cnt = (repo.Item1 as IRepository).Count();
            Writer.WriteLine(cnt + " elements in " + name);
            return null;
        }

        [CommandDescription("Deleta all from table by name")]
        public string[] Truncate(string name)
        {
            if (name == null)
            {
                return ConsoleConfigSection.GetConfig().Clean
                    .Cast<NameValueConfigElement>()
                    .Select(x => x.Name).ToArray();
            }

            var stringType = ConsoleConfigSection.GetConfig().Clean
                .Cast<NameValueConfigElement>().First(x => x.Name == name).Value;

            var type = Type.GetType(stringType);
            if (type == null) throw new Exception(stringType + " can not be resolved.");

            var repoFinder = Context.Resolve<RepositoryFinder>();
            var repo = repoFinder.CreateRepository(type);

            (repo.Item1 as IRepository).Truncate();
            Writer.WriteLine(name + " truncated");
            return null;
        }

        [CommandDescription("Restore table from xml file")]
        public string[] Restore(string name)
        {
            if (name == null)
            {
                return ConsoleConfigSection.GetConfig().Restore
                    .Cast<NameValueConfigElement>()
                    .Select(x => x.Name).ToArray();
            }

            var filename = String.Format(@"Restore{1}{0}.xml", name, Path.DirectorySeparatorChar);
            var stringType = ConsoleConfigSection.GetConfig().Restore
                .Cast<NameValueConfigElement>().First(x => x.Name == name).Value;

            var type = Type.GetType(stringType);
            if (type == null) throw new Exception(stringType + " can not be resolved.");
            if (File.Exists(filename))
            {
                INHibernateFileBackupBase fileBackup = Context.Resolve(type) as INHibernateFileBackupBase;
                fileBackup.BackupFile = filename;
                fileBackup.ImportStrategy = BoxImporterStrategy.SkipExisting;
                fileBackup.Restore();
            }
            else
            {
                Writer.WriteLine(filename + " not found");
            }
            return null;
        }

        [CommandDescription("Export table to xml file")]
        public string[] Export(string name)
        {
            if (name == null)
            {
                return ConsoleConfigSection.GetConfig().Restore
                    .Cast<NameValueConfigElement>()
                    .Select(x => x.Name).ToArray();
            }

            var filename = String.Format(@"Restore{1}{0}_exported.xml", name, Path.DirectorySeparatorChar);
            var stringType = ConsoleConfigSection.GetConfig().Restore
                .Cast<NameValueConfigElement>().First(x => x.Name == name).Value;

            var type = Type.GetType(stringType);
            if (type == null) throw new Exception(stringType + " can not be resolved.");
            if (File.Exists(filename))
            {
                File.Delete(filename);

            }
            INHibernateFileBackupBase fileBackup = Context.Resolve(type) as INHibernateFileBackupBase;
            fileBackup.BackupFile = filename;
            fileBackup.Backup();
            return null;            
        }

        //public static List<Tuple<string, Type>> KnownTypes = new List<Tuple<string, Type>>  {
        //    new Tuple<string, Type>("HttpProxy", typeof(HttpProxyEntity)),
        //    new Tuple<string, Type>("Schedule", typeof(ScheduleEntity))
        //};
        //void BoxBackup<T>(string path) where T : Entity
        //{
        //    var dbContext = Context.Resolve<IDbContext>();
        //    using (var sess = dbContext.SessionFactory.OpenSession())
        //    {
        //        var box = sess.CreateCriteria(typeof(T))
        //            .List().Cast<T>().ToBox();
        //        box.ToXml().ToFile(path);
        //    }
        //}
        //void BoxRestore<T>(string path, BoxImporterStrategy strategy = BoxImporterStrategy.SkipExisting) where T : Entity
        //{
        //    var importer = Context.Resolve<BoxImporter<T>>();
        //    var presentation = new XmlBoxPresentation<T>();
        //    var box = presentation.FromFile(path);
        //    importer.ImportBox(box, BoxImporterStrategy.SkipExisting);
        //}

    }
}
