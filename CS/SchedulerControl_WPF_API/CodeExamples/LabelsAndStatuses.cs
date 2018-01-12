using DevExpress.Xpf.Scheduler;
using DevExpress.XtraScheduler;
using System;
using System.Windows.Media;

namespace SchedulerControl_WPF_API {
    class LabelsAndStatuses
    {
        static void CustomLabelsAndStatusesAction(SchedulerControl scheduler)
        {
            #region #CustomLabelsAndStatuses
            scheduler.Storage.AppointmentStorage.Clear();

            string[] IssueList = { "Consultation", "Treatment", "X-Ray" };
            Color[] IssueColorList = { Colors.Ivory, Colors.Pink, Colors.Plum };
            string[] PaymentStatuses = { "Paid", "Unpaid" };
            Color[] PaymentColorStatuses = { Colors.Green, Colors.Red };


            IAppointmentLabelStorage labelStorage = scheduler.Storage.AppointmentStorage.Labels;
            labelStorage.Clear();
            int count = IssueList.Length;
            for (int i = 0; i < count; i++)
            {
                IAppointmentLabel label = labelStorage.CreateNewLabel(i, IssueList[i]);
                label.SetColor(IssueColorList[i]);
                labelStorage.Add(label);
            }
            IAppointmentStatusStorage statusStorage = scheduler.Storage.AppointmentStorage.Statuses;
            statusStorage.Clear();
            count = PaymentStatuses.Length;
            for (int i = 0; i < count; i++)
            {
                IAppointmentStatus status = statusStorage.CreateNewStatus(i, PaymentStatuses[i], PaymentStatuses[i]);
                status.SetBrush(new SolidColorBrush(PaymentColorStatuses[i]));
                statusStorage.Add(status);
            }

            // Create a new appointment.
            Appointment apt = scheduler.Storage.CreateAppointment(AppointmentType.Normal);
            apt.Subject = "Test";
            apt.Start = DateTime.Now;
            apt.End = DateTime.Now.AddHours(2);
            apt.ResourceId = scheduler.Storage.ResourceStorage[0].Id;
            apt.LabelKey = labelStorage.GetByIndex(2).Id;
            scheduler.Storage.AppointmentStorage.Add(apt);

            #endregion #CustomLabelsAndStatuses
        }
    }
}
