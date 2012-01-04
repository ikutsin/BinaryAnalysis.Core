using System.Windows;

namespace BA.Examples.ScriptingHelper.Models
{
    //public class TreeItemTagInfo : TagInfo
    //{
    //    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(InputTagInfo));
    //    public XElement Value
    //    {
    //        get { return (string)GetValue(ValueProperty); }
    //        set { SetValue(ValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children",
    //        typeof(ObservableCollection<TreeItemTagInfo>), typeof(TreeItemTagInfo));
    //    private ObservableCollection<TreeItemTagInfo> Children
    //    {
    //        get { return (ObservableCollection<TreeItemTagInfo>)GetValue(ChildrenProperty); }
    //        set { SetValue(ChildrenProperty, value); }
    //    }
    //}
    public class InputTagInfo : TagInfo
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(InputTagInfo));
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(InputTagInfo));
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(InputTagInfo));
        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
    }
    public class TagInfo : DependencyObject
    {
        public static readonly DependencyProperty TagNameProperty = DependencyProperty.Register("TagName", typeof(string), typeof(TagInfo));
        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        public override string ToString()
        {
            return TagName;
        }
    }
}
