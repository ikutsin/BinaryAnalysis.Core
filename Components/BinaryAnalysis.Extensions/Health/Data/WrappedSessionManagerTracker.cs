using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.SessionManagement;
using BinaryAnalysis.Extensions.Health.Data;
using log4net;

namespace BinaryAnalysis.Extensions.Health
{
    public class WrappedSessionManagerTracker : ISessionManager
    {
        private readonly TrackedModuledDbContext _trackableContext;
        private readonly ISessionManager _parent;

        public WrappedSessionManagerTracker(TrackedModuledDbContext trackableContext, ISessionManager parent)
        {
            _trackableContext = trackableContext;
            _parent = parent;
        }

        #region Implementation of ISessionManager

        public void Init(IDbContext context)
        {
            _parent.Init(context);
        }

        public DbWorkUnit WorkUnitFor(object repo, DbWorkUnitType type)
        {
            var unit = _parent.WorkUnitFor(repo, type);
            switch (type)
            {
                case DbWorkUnitType.Write:
                    _trackableContext.FreqWrite.Notify();
                    break;
                case DbWorkUnitType.Read:
                default:
                    _trackableContext.FreqRead.Notify();
                    break;
            }
            return unit;
        }

        public void NotifyRepoCreated(object repo)
        {
            _parent.NotifyRepoCreated(repo);
        }

        public void NotifyRepoDisposed(object repo)
        {
            _parent.NotifyRepoDisposed(repo);
        }

        #endregion
    }
}
