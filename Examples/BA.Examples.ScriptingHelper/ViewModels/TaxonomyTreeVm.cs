using System;
using System.Collections.ObjectModel;
using System.Windows;
using BA.Examples.ScriptingHelper.Models;
using BinaryAnalysis.Data;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class TaxonomyTreeVm : AbstractVm
    {
        private TaxonomyHierarchy tree;
        public TaxonomyHierarchy Tree
        {
            get { return tree; }
            set { tree = value; NotifyPropertyChanged("Tree"); }
        }

        private ObservableCollection<string> relations;
        public ObservableCollection<string> Relations
        {
            get { return relations; }
            set { relations = value; NotifyPropertyChanged("Relations"); }
        }

        private ObservableCollection<string> classifications;
        public ObservableCollection<string> Classifications
        {
            get { return classifications; }
            set { classifications = value; NotifyPropertyChanged("Classifications"); }
        }

        public void LoadRelations(TaxonomyNodeBoxMap taxon)
        {
            try
            {
                Classifications = new ObservableCollection<string>(TaxonomyHierarchy.ServiceClient.GetClassified(taxon));
                Relations = new ObservableCollection<string>(TaxonomyHierarchy.ServiceClient.GetRelationsByType(taxon));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error loading Relations");
            }
        }

        public TaxonomyTreeVm()
        {
            try
            {
                Tree = new TaxonomyHierarchy(TaxonomyHierarchy.ServiceClient.GetRoot());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error loading Root");
            }
            Relations = new ObservableCollection<string>();
            Classifications = new ObservableCollection<string>();
        }
    }
}
