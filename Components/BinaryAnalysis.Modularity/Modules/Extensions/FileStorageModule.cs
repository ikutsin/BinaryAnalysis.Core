using Autofac;
using BinaryAnalysis.Extensions.FileStorage;

namespace BinaryAnalysis.Modularity.Modules.Extensions
{
    public class FileStorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //repo
            builder.RegisterType<FileRepository>()
                .WithParameter("folderName", "files")
                .SingleInstance();
        }
    }
}
