using DevExpress.XtraScheduler;
using System;

namespace SchedulerControl_WPF_API {
    class AppointmentConflicts
    {
        static void AllowAppointmentConflictsEvent(DevExpress.Xpf.Scheduler.SchedulerControl scheduler) {
            #region #AllowAppointmentConflictsEvent
            //Concurrent appointments with the same resource are not allowed.
           scheduler.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Custom;
            scheduler.AllowAppointmentConflicts += Scheduler_AllowAppointmentConflicts;

            scheduler.Storage.AppointmentStorage.Clear();
            scheduler.GroupType = SchedulerGroupType.Resource;
            Appointment apt1 = scheduler.Storage.AppointmentStorage.CreateAppointment(AppointmentType.Normal, DateTime.Now, DateTime.Now.AddHours(2));
            apt1.ResourceId = scheduler.Storage.ResourceStorage[0].Id;
            scheduler.Storage.AppointmentStorage.Add(apt1);
            Appointment apt2 = scheduler.Storage.AppointmentStorage.CreateAppointment(AppointmentType.Normal, DateTime.Now, DateTime.Now.AddHours(2));
            apt2.ResourceId = scheduler.Storage.ResourceStorage[1].Id;
            scheduler.Storage.AppointmentStorage.Add(apt2);
            #endregion #AllowAppointmentConflictsEvent
        }
        #region #@AllowAppointmentConflictsEvent
        static void Scheduler_AllowAppointmentConflicts(object sender, AppointmentConflictEventArgs e) {
            e.Conflicts.Clear();
            FillConflictedAppointmentsCollection(e.Conflicts, e.Interval, ((DevExpress.Xpf.Scheduler.SchedulerControl)sender).Storage.AppointmentStorage.Items, e.AppointmentClone);
        }
        static void FillConflictedAppointmentsCollection(AppointmentBaseCollection conflicts, TimeInterval interval,
            AppointmentBaseCollection collection, Appointment currApt) {
            for (int i = 0; i < collection.Count; i++) {
                Appointment apt = collection[i];
                if (new TimeInterval(apt.Start, apt.End).IntersectsWith(interval) & !(apt.Start == interval.End || apt.End == interval.Start)) {
                    if (apt.ResourceId == currApt.ResourceId) {
                        conflicts.Add(apt);
                    }
                }
                if (apt.Type == AppointmentType.Pattern) {
                    FillConflictedAppointmentsCollection(conflicts, interval, apt.GetExceptions(), currApt);
                }
            }
        }
        #endregion #@AllowAppointmentConflictsEvent

    }
}
