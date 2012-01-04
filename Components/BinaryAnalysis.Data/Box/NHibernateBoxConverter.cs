using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinaryAnalysis.Box;
using BinaryAnalysis.Data.Core;
using BinaryAnalysis.Data.Core.Impl;

namespace BinaryAnalysis.Data.Box
{
    public class NHibernateBoxConverter<TE, TM>
        where TM:EntityBoxMap
        where TE:Entity
    {
        private RepositoryFinder _repoFinder;

        public NHibernateBoxConverter(RepositoryFinder _repoFinder)
        {
            this._repoFinder = _repoFinder;
        }

        public static List<string> PropertiesToSkip =
            new List<string> {"Classifications", "Entries", "Metrics", "Settings"};

        public TM ToBox(TE e)
        {
            TM eMap = (TM)Activator.CreateInstance(typeof(TM));
            var allProps = eMap.GetType().GetProperties()
                .Select(p => new
                {
                    attr =
                        p.GetCustomAttributes(true).Where(x => x is ManualBoxingAttribute).FirstOrDefault(),
                    prop = p
                }).Where(a => !PropertiesToSkip.Contains(a.prop.Name));

            //standard mapping
            var standadProps = allProps.Where(a => a.attr == null);
            foreach (var mapProp in standadProps.Select(a=>a.prop))
            {
                var eProp = typeof (TE).GetProperty(mapProp.Name);
                if (eProp != null)
                {
                    MapProperty(e, eMap, eProp, mapProp);
                }
            }
            
            //manual mapping
            var manualProps = allProps.Where(a => a.attr != null);
            manualProps.Where(a => a.attr is ManualBoxingReferenceAttribute).ToList().ForEach(a =>
            {
                var refEntity = (Entity)typeof(TE).GetProperty(a.prop.Name).GetValue(e, null);
                if (refEntity != null) a.prop.SetValue(eMap, refEntity.Id, null);
            });
            manualProps.Where(a => a.attr is ManualBoxingMethodAttribute).ToList().ForEach(a =>
            {
                var method = e.GetType().GetMethod(((ManualBoxingMethodAttribute)a.attr).BoxMethodName);
                method.Invoke(null, new object[] { e, eMap });
            });
            return eMap;
        }

        protected void MapProperty(object source, object dest, 
            PropertyInfo sProp, PropertyInfo dProp)
        {
            if (dProp.CanWrite && sProp.CanRead)
            {
                if (sProp.PropertyType == dProp.PropertyType)
                {
                    dProp.SetValue(dest, sProp.GetValue(source, null), null);
                }
                else if (sProp.PropertyType == typeof (string) && dProp == typeof (Uri))
                {
                    dProp.SetValue(dest, new Uri((string) sProp.GetValue(source, null)), null);
                }
                else
                {
                    throw new Exception(String.Format("Unknown mapping: {0}->{1} of {2}({3}->{4})",
                                                      source.GetType().Name, dest.GetType().Name,
                                                      sProp.Name, sProp.PropertyType.Name, dProp.PropertyType.Name));
                }
            }
        }

        public TE ToEntity(TM eMap)
        {
            TE e = (TE)Activator.CreateInstance(typeof(TE));


            var allProps = eMap.GetType().GetProperties()
                .Select(p => new
                {
                    attr =
                p.GetCustomAttributes(true).Where(x => x is ManualBoxingAttribute).FirstOrDefault(),
                    prop = p
                }).Where(a => !PropertiesToSkip.Contains(a.prop.Name));

            //standard mapping
            var standadProps = allProps.Where(a => a.attr == null);
            foreach (var mapProp in standadProps.Select(a => a.prop))
            {
                var eProp = typeof(TE).GetProperty(mapProp.Name);
                if (eProp != null)
                {
                    MapProperty(eMap, e, mapProp,eProp);
                }
            }

            //manual mapping
            var manualProps = allProps.Where(a => a.attr != null);
            manualProps.Where(a => a.attr is ManualBoxingReferenceAttribute).ToList().ForEach(a =>
            {
                var id = (int)a.prop.GetValue(eMap, null);
                var eprop = typeof(TE).GetProperty(a.prop.Name);
                if (id > 0)
                {
                    var repoTuple = _repoFinder.CreateRepository(eprop.PropertyType);
                    var refEntity = repoTuple.Item2.GetMethod("Get").Invoke(repoTuple.Item1, new object[] { id, Enums.LockMode.None });
                    eprop.SetValue(e, refEntity, null);
                }
                else
                {
                    eprop.SetValue(e, null, null);
                }
            });

            manualProps.Where(a => a.attr is ManualBoxingMethodAttribute).ToList().ForEach(a =>
            {
                var method = e.GetType().GetMethod(((ManualBoxingMethodAttribute)a.attr).UnboxMethodName);
                method.Invoke(null, new object[] { e, eMap });
            });

            return e;
        }
        #region Collections
        public IBox<TM> ToBox(IEnumerable<TE> list)
        {
            Box<TM> box = new Box<TM>();
            foreach (var e in list)
            {
                box.Add(ToBox(e));
            }
            return box;
        }

        public IList<TE> ToEntity(IBox<TM> box)
        {
            List<TE> entities = new List<TE>();
            foreach (var emap in box)
            {
                entities.Add(ToEntity((TM)emap));
            }
            return entities;
        } 
        #endregion

    }
}
