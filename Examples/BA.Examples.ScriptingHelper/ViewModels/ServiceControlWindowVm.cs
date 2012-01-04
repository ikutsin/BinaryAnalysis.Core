using System;
using Autofac;
using BA.Examples.ServiceProcess.Services;

namespace BA.Examples.ScriptingHelper.ViewModels
{
    public class ServiceControlWindowVm : AbstractVm
    {
        public ServiceControlWindowVm()
        {
            ScheduleTime = DateTime.Now;
        }

        private DateTime scheduleTime;
        public DateTime ScheduleTime
        {
            get { return scheduleTime; }
            set { scheduleTime = value; NotifyPropertyChanged("ScheduleTime"); }
        }

        private ISchedulerService serviceClient;
        public ISchedulerService ServiceClient
        {
            get
            {
                if(serviceClient==null)
                {
                    serviceClient = App.CurrentApp.Container.Resolve<ISchedulerService>();
                }
                return serviceClient;
            }
        }
  
    }
}
