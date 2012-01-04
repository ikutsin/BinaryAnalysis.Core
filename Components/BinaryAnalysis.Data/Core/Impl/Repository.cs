using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Reflection;
using NHibernate.Validator.Engine;
using BinaryAnalysis.Data.Core.SessionManagement;
using log4net;
using BinaryAnalysis.Data.Box;

namespace BinaryAnalysis.Data.Core.Impl
{
    public class Repository<T> : RepositoryWithTypedId<T, int>, IRepository<T> where T:class
    {
        public Repository(IDbContext context, ILog log) : base(context) 
        {
            this.log = log;
        }
        protected ILog log { get; set; }
        public int Count()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof (T));
                criteria.SetProjection(Projections.RowCount());
                return criteria.UniqueResult<int>();
            }
        }

        public void Truncate()
        {
            DeleteAll(GetAll());
        }
    }

    public class RepositoryWithTypedId<T, IdT> : IRepositoryWithTypedId<T, IdT>
    {
        protected ISessionManager SessionManager { get; set; }
        protected ValidatorEngine Validator { get; set; }

        public RepositoryWithTypedId(IDbContext context)
        {
            if (context == null || context.SessionManager == null)
                throw new InvalidOperationException("context or its session manager is not initialized");

            SessionManager = context.SessionManager;
            Validator = context.ValidatorEngine;
            SessionManager.NotifyRepoCreated(this);
        }

        public virtual IList<T> GetAll()
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof(T));
                return criteria.List<T>();
            }
        }

        public virtual IList<T> FindAll(IDictionary<string, object> propertyValuePairs)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                if(propertyValuePairs == null && propertyValuePairs.Count == 0)
                    throw new InvalidOperationException(
                    "propertyValuePairs was null or empty; " +
                    "it has to have at least one property/value pair in it");

                ICriteria criteria = wu.Session.CreateCriteria(typeof(T));

                foreach (string key in propertyValuePairs.Keys)
                {
                    if (propertyValuePairs[key] != null)
                    {
                        criteria.Add(Expression.Eq(key, propertyValuePairs[key]));
                    }
                    else
                    {
                        criteria.Add(Expression.IsNull(key));
                    }
                }

                var ret = criteria.List<T>();
                return ret;
            }
        }

        public virtual T FindOne(IDictionary<string, object> propertyValuePairs)
        {
            IList<T> foundList = FindAll(propertyValuePairs);

            if (foundList.Count > 1)
            {
                throw new NonUniqueResultException(foundList.Count);
            }
            else if (foundList.Count == 1)
            {
                return foundList[0];
            }

            return default(T);
        }

        public virtual T Get(IdT id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return wu.Session.Get<T>(id, ConvertFrom(lockMode));
            }
        }
        public virtual T Load(IdT id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                return wu.Session.Load<T>(id, ConvertFrom(lockMode));
            }
        }


        public virtual IList<T> FindAll(T exampleInstance, params string[] propertiesToExclude)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria criteria = wu.Session.CreateCriteria(typeof(T));
                Example example = Example.Create(exampleInstance);

                foreach (string propertyToExclude in propertiesToExclude)
                {
                    example.ExcludeProperty(propertyToExclude);
                }

                criteria.Add(example);

                return criteria.List<T>();
            }
        }

        public virtual T FindOne(T exampleInstance, params string[] propertiesToExclude)
        {
            IList<T> foundList = FindAll(exampleInstance, propertiesToExclude);

            if (foundList.Count > 1)
            {
                throw new NonUniqueResultException(foundList.Count);
            }
            else if (foundList.Count == 1)
            {
                return foundList[0];
            }

            return default(T);
        }

        /// <summary>
        /// Merges before
        /// http://stackoverflow.com/questions/170962/nhibernate-difference-between-session-merge-and-session-saveorupdate
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                entity = (T)wu.Session.Merge(entity);
                wu.Session.Delete(entity);
            }            
        }

        public TT CriteriaUnique<TT>(Action<ICriteria> criteria)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria c = wu.Session.CreateCriteria(typeof(T));
                criteria.Invoke(c);
                return c.UniqueResult<TT>();
            }
        }

        public IList CriteriaList(Action<ICriteria> criteria)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Read))
            {
                ICriteria c = wu.Session.CreateCriteria(typeof(T));
                criteria.Invoke(c);
                return c.List();
            }
        }

        public void ActionCommand(Action<ISession> action)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                action(wu.Session);
            }
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            foreach (var e in entities) Delete(e);
        }

        /// <summary>
        /// Although SaveOrUpdate _can_ be invoked to update an object with an assigned Id, you are 
        /// hereby forced instead to use Save/Update for better clarity.
        /// </summary>
        public virtual T SaveOrUpdate(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                wu.Session.SaveOrUpdate(entity);
                return entity;
            }
        }


        public virtual T Save(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                wu.Session.Save(entity);
                return entity;
            }
        }
        /// <summary>
        /// Merges before update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T Update(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                entity = (T)wu.Session.Merge(entity);
                wu.Session.Update(entity);
                return entity;
            }
        }
        public void UpdateAll(IEnumerable<T> entities)
        {
            foreach (var e in entities) Update(e);
        }

        public virtual void Evict(T entity)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                wu.Session.Evict(entity);
            }
        }

        /// <summary>
        /// Translates a domain layer lock mode into an NHibernate lock mode via reflection.  This is 
        /// provided to facilitate developing the domain layer without a direct dependency on the 
        /// NHibernate assembly.
        /// </summary>
        protected LockMode ConvertFrom(Enums.LockMode lockMode)
        {
            FieldInfo translatedLockMode = typeof(LockMode).GetField(lockMode.ToString(),
                BindingFlags.Public | BindingFlags.Static);

            if(translatedLockMode == null) throw new InvalidCastException(
                    "The provided lock mode , '" + lockMode + ",' " +
                    "could not be translated into an NHibernate.LockMode. This is probably because " +
                    "NHibernate was updated and now has different lock modes which are out of synch " +
                    "with the lock modes maintained in the domain layer.");

            return (LockMode)translatedLockMode.GetValue(null);
        }

        public void Dispose()
        {
            SessionManager.NotifyRepoDisposed(this);
        }

        internal protected IQueryable<T> AsQueryable(ISession session)
        {
            return session.Query<T>();
        }

        public ComponentsLoadLevel ComponentsLoadLevel { get; set; }

        public virtual bool IsValid(T entity)
        {
            return Validator.IsValid(entity);
        }
        public virtual IList<InvalidValue> Validate(T entity)
        {
            return Validator.Validate(entity);
        }


        public virtual object ExecuteDetachedExpression(System.Linq.Expressions.Expression expression, Type elementType, Type ienumerableExpressionType)
        {
            using (var wu = SessionManager.WorkUnitFor(this, DbWorkUnitType.Write))
            {
                var qProvider = new NhQueryProvider(wu.Session);

                //var query = qProvider.CreateQuery<T>(expression);
                var result = qProvider.Execute(expression);
                return result;
                
            }
        }

    }
}
