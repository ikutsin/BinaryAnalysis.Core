using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Validator.Engine;
using BinaryAnalysis.Data.Core.SessionManagement;
using NHibernate.Cfg;

namespace BinaryAnalysis.Data.Core
{
    public interface IDbContext
    {
        SchemaExport GetSchemaExport();
        void ValidateSchema();
        ValidatorEngine ValidatorEngine { get; }
        ISessionFactory SessionFactory { get; }
        ISessionManager SessionManager { get; }
        Configuration CurrentConfiguration { get; }
    }
}
