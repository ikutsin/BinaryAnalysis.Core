using System;
using System.Collections.Generic;
using BinaryAnalysis.Box.Transforations;
using BinaryAnalysis.Data;
using BinaryAnalysis.Data.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Extensions.FileStorage;
using log4net;

namespace BinaryAnalysis.Extensions.HttpProxy.Data
{
    public class HttpProxyFileBackup : NHibernateFileBackupBase<HttpProxyBoxMap, HttpProxyEntity>
    {
        public HttpProxyFileBackup(
            FileBoxTransformation<HttpProxyBoxMap> fileTransform, 
            NHibernateBoxTransformation<HttpProxyBoxMap, HttpProxyEntity> dbTransform) : base(fileTransform, dbTransform)
        {
            dbTransform.FindExistingEntity = (repo, e) => repo.FindOne(new Dictionary<string, object> { { "IP", e.IP } });
        }
    }
}
