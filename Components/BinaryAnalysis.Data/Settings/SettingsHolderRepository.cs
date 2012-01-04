using System.Collections.Generic;
using BinaryAnalysis.Data.Classification;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;
using log4net;

namespace BinaryAnalysis.Data.Settings
{
    public class SettingsHolderRepository<T> : ClassifiableRepository<T> where T : Entity, ISettingsHolder
    {
        protected SettingsService settingsService;
        public SettingsHolderRepository(IDbContext context, ILog log, 
            SettingsService settingsService, RelationService relRepo)
            :base(context,log, relRepo)
        {
            this.settingsService = settingsService;
            ComponentsLoadLevel = ComponentsLoadLevel.Single;
        }

        public override void Delete(T entity)
        {
            settingsService.DeleteFor(entity);
            base.Delete(entity);
        }

        public void LoadSettingsIfNull(T entity)
        {
            if (entity!=null && entity.Settings == null) entity.Settings = settingsService.GetFor(entity);
        }
        public void LoadSettingsIfNull(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                LoadSettingsIfNull(entity);
            }
        }

        public override T Get(int id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            var entity = base.Get(id, lockMode);
            if (ComponentsLoadLevel >= ComponentsLoadLevel.Single && entity != null)
            {
                entity.Settings = settingsService.GetFor(entity);
            }
            return entity;
        }
        public override T Load(int id, Enums.LockMode lockMode = Enums.LockMode.None)
        {
            var entity = base.Load(id, lockMode);
            if (ComponentsLoadLevel >= ComponentsLoadLevel.Single && entity != null)
            {
                entity.Settings = settingsService.GetFor(entity);
            }
            return entity;
        }

        public override T SaveOrUpdate(T entity)
        {
            var e = base.SaveOrUpdate(entity);
            if (ComponentsLoadLevel >= ComponentsLoadLevel.Single || entity.Settings != null)
            {
                settingsService.SaveFor(e, entity.Settings);
            }
            return e;
        }
        public override T Save(T entity)
        {
            base.Save(entity);
            if (ComponentsLoadLevel >= ComponentsLoadLevel.Single || entity.Settings != null)
            {
                settingsService.SaveFor(entity, entity.Settings);
            }
            return entity;
        }
        public override T Update(T entity)
        {
            if (ComponentsLoadLevel >= ComponentsLoadLevel.Single || entity.Settings != null)
            {
                settingsService.SaveFor(entity, entity.Settings);
            }
            base.Update(entity);
            return entity;
        }

        public override object ExecuteDetachedExpression(System.Linq.Expressions.Expression expression, System.Type elementType, System.Type ienumerableExpressionType)
        {
            var result = base.ExecuteDetachedExpression(expression, elementType, ienumerableExpressionType);
            if (result is IEnumerable<T>)
            {
                foreach (T entity in result as IEnumerable<T>)
                {
                    if (ComponentsLoadLevel >= ComponentsLoadLevel.Always)
                    {
                        entity.Settings = settingsService.GetFor(entity);
                    }
                }
            }
            else if (result is T)
            {
                if (ComponentsLoadLevel >= ComponentsLoadLevel.Single)
                {
                    settingsService.GetFor(result as T);
                }
            }
            return result;
        }

    }
}
