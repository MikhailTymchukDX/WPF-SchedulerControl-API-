using DevExpress.Xpf.Scheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchedulerControl_WPF_API {
    /// <summary>
    /// Interaction logic for DisplayResultControl.xaml
    /// </summary>
    public partial class DisplayResultControl : UserControl {
        private BindingList<CustomResource> CustomResourceCollection = new BindingList<CustomResource>();
        private BindingList<CustomAppointment> CustomEventList = new BindingList<CustomAppointment>();

        public SchedulerControl SchedulerControl { get; set; }

        public DisplayResultControl() {
            InitializeComponent();
            this.SchedulerControl = this.scheduler;

            InitializeScheduler();
        }

        private void InitializeScheduler() {
            InitHelper.InitResources(CustomResourceCollection);
            InitHelper.InitAppointments(CustomEventList, CustomResourceCollection);

            ResourceMapping mappingsResource = this.scheduler.Storage.ResourceStorage.Mappings;
            mappingsResource.Id = "ResID";
            mappingsResource.Caption = "Name";

            AppointmentMapping mappingsAppointment = this.scheduler.Storage.AppointmentStorage.Mappings;
            mappingsAppointment.Start = "StartTime";
            mappingsAppointment.End = "EndTime";
            mappingsAppointment.Subject = "Subject";
            mappingsAppointment.AllDay = "AllDay";
            mappingsAppointment.Description = "Description";
            mappingsAppointment.Label = "Label";
            mappingsAppointment.Location = "Location";
            mappingsAppointment.RecurrenceInfo = "RecurrenceInfo";
            mappingsAppointment.ReminderInfo = "ReminderInfo";
            mappingsAppointment.ResourceId = "OwnerId";
            mappingsAppointment.Status = "Status";
            mappingsAppointment.Type = "EventType";

            this.scheduler.Storage.BeginUpdate();
            this.scheduler.Storage.ResourceStorage.DataSource = CustomResourceCollection;
            this.scheduler.Storage.AppointmentStorage.DataSource = CustomEventList;
            this.scheduler.Storage.EndUpdate();

            this.scheduler.Start = DateTime.Now;
            this.scheduler.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Day;
        }

        public void Reset() {
            ClearScheduler();
            CustomResourceCollection.Clear();
            CustomEventList.Clear();
            InitializeScheduler();
        }

        private void ClearScheduler() {
            this.scheduler.Storage.BeginUpdate();
            this.scheduler.Storage.ResourceStorage.Mappings = new ResourceMapping();
            this.scheduler.Storage.AppointmentStorage.Mappings = new AppointmentMapping();
            this.scheduler.Storage.ResourceStorage.DataSource = null;
            this.scheduler.Storage.AppointmentStorage.DataSource = null;
            this.scheduler.Storage.EndUpdate();
        }
    }
}
