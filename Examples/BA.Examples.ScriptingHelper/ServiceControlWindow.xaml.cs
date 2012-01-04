using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Autofac;
using BA.Examples.ScriptingHelper.Models;
using BA.Examples.ScriptingHelper.ViewModels;
using BA.Examples.ServiceProcess.Services;
using BinaryAnalysis.Scheduler.Scheduler.Data;

namespace BA.Examples.ScriptingHelper
{
    /// <summary>
    /// Interaction logic for ServiceControlWindow.xaml
    /// </summary>
    public partial class ServiceControlWindow : Window
    {
        public ServiceControlWindow()
        {
            InitializeComponent();
            SchedulesGrid.ListView.SelectionChanged += (s, e) =>
            {
                ScheduleBoxMap scheduleBoxMap = (SchedulesGrid.ListView.SelectedItem as ScheduleBoxMap);
                if(scheduleBoxMap!=null)SettingGrid.ViewModel.SetItem(scheduleBoxMap.Settings);
            };
        }

        public ServiceControlWindowVm ViewModel
        {
            get { return DataContext as ServiceControlWindowVm;  }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (tabControl.SelectedItem as TabItem);
            if(item==null) return;
            if (item == tabHistory)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(SchedulesGrid.ViewModel.LoadItems));
            }
            else if (item == tabScheduling)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(()=>
                {
                    try
                    {
                        var schedulerClient = App.CurrentApp.Container.Resolve<ISchedulerService>();
                        var goals = schedulerClient.GetAvailableNamedGoals();
                        availableSchedulesGrid.ViewModel.Items.Clear();
                        int cnt = 0;
                        foreach (var goal in goals)
                        {
                            availableSchedulesGrid.ViewModel.Items.Add(new NameValueItem { Name = cnt.ToString(), Value = goal });
                            cnt++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error connecting to server");
                    }

                }));
            }
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (availableSchedulesGrid.ViewModel.SelectedName != null)
            {
                MessageBox.Show("Scheduling "
                                + availableSchedulesGrid.ViewModel.SelectedName + " at " 
                                + ViewModel.ScheduleTime);

            }
        }

        private void NowButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ScheduleTime = DateTime.Now;
        }
    }
}
