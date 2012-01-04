using System;
using System.Collections.Generic;
using System.Windows;
using Autofac;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Data;

namespace BA.Examples.ScriptingHelper.Models
{
    public class TaxonomyHierarchy : ObservableHierarchy<TaxonomyNodeBoxMap>
    {
        private static ICommonsService serviceClient;
        public static ICommonsService ServiceClient
        {
            get { return serviceClient ?? (serviceClient = App.CurrentApp.Container.Resolve<ICommonsService>()); }
        }

        public TaxonomyHierarchy(TaxonomyNodeBoxMap current)
            : base(current)
        {
        }

        protected override IEnumerable<TaxonomyNodeBoxMap> Expand()
        {
            try
            {
                return ServiceClient.ChildrenOf(Current);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading choldren");
            }
            return null;
        }

        protected override ObservableHierarchy<TaxonomyNodeBoxMap> Create(TaxonomyNodeBoxMap item)
        {
            return new TaxonomyHierarchy(item);
        }
    }
}
