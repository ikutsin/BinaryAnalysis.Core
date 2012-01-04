using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Validator.Engine;
using BinaryAnalysis.Data.Box;
using System.Linq.Expressions;

namespace BinaryAnalysis.Data.Core
{
    public interface IRepository
    {
        int Count();
        void Truncate();

        object ExecuteDetachedExpression(
            Expression expression, Type elementType, Type ienumerableExpressionType);
    }

    public interface IRepository<T> : IRepository, IRepositoryWithTypedId<T, int> where T:class { }

    public interface IRepositoryWithTypedId<T, IdT> : IDisposable
    {
        ComponentsLoadLevel ComponentsLoadLevel { get; set; }

        bool IsValid(T obj);
        IList<InvalidValue> Validate(T entity);
       
        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        T Get(IdT id, Enums.LockMode lockMode = Enums.LockMode.None);

        /// <summary>
        /// Throws an exception if a row is not found matching the provided Id.
        /// </summary>
        T Load(IdT id, Enums.LockMode lockMode = Enums.LockMode.None);

        /// <summary>
        /// Looks for zero or more instances using the example provided.
        /// </summary>
        IList<T> FindAll(T exampleInstance, params string[] propertiesToExclude);

        /// <summary>
        /// Looks for a single instance using the example provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        T FindOne(T exampleInstance, params string[] propertiesToExclude);

        /// <summary>
        /// Returns all of the items of a given type.
        /// </summary>
        IList<T> GetAll();

        /// <summary>
        /// Looks for zero or more instances using the <see cref="IDictionary{TKey,TValue}"/> provided.
        /// The key of the collection should be the property name and the value should be
        /// the value of the property to filter by.
        /// </summary>
        IList<T> FindAll(IDictionary<string, object> propertyValuePairs);

        /// <summary>
        /// Looks for a single instance using the property/values provided.
        /// </summary>
        /// <exception cref="NonUniqueResultException" />
        T FindOne(IDictionary<string, object> propertyValuePairs);


        /// <summary>
        /// For entities that have assigned Id's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/nhibernate/html_single/#mapping-declaration-id-assigned.
        /// </summary>
        T Save(T entity);

        /// <summary>
        /// For entities that have assigned Id's, you should explicitly call Update to update an existing one.
        /// Updating also allows you to commit changes to a detached object.  More info may be found at:
        /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
        /// </summary>
        T Update(T entity);
        void UpdateAll(IEnumerable<T> entities);

        /// <summary>
        /// Dissasociates the entity with the ORM so that changes made to it are not automatically 
        /// saved to the database.  More precisely, this removes the entity from <see cref="ISession" />'s cache.
        /// More details may be found at http://www.hibernate.org/hib_docs/nhibernate/html_single/#performance-sessioncache.
        /// </summary>
        void Evict(T entity);

        /// <summary>
        /// For entities with automatatically generated Ids, such as identity, SaveOrUpdate may 
        /// be called when saving or updating an entity.
        /// 
        /// Updating also allows you to commit changes to a detached object.  More info may be found at:
        /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
        /// </summary>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// I'll let you guess what this does.
        /// </summary>
        void Delete(T entity);

        TT CriteriaUnique<TT>(Action<ICriteria> criteria);
        IList CriteriaList(Action<ICriteria> criteria);
        void ActionCommand(Action<ISession> action);
    }
}
