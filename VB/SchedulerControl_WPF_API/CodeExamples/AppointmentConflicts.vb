Imports DevExpress.XtraScheduler
Imports System

Namespace SchedulerControl_WPF_API
    Friend Class AppointmentConflicts
        Private Shared Sub AllowAppointmentConflictsEvent(ByVal scheduler As DevExpress.Xpf.Scheduler.SchedulerControl)
'            #Region "#AllowAppointmentConflictsEvent"
            'Concurrent appointments with the same resource are not allowed.
           scheduler.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Custom
            AddHandler scheduler.AllowAppointmentConflicts, AddressOf Scheduler_AllowAppointmentConflicts

            scheduler.Storage.AppointmentStorage.Clear()
            scheduler.GroupType = SchedulerGroupType.Resource
            Dim apt1 As Appointment = scheduler.Storage.AppointmentStorage.CreateAppointment(AppointmentType.Normal, Date.Now, Date.Now.AddHours(2))
            apt1.ResourceId = scheduler.Storage.ResourceStorage(0).Id
            scheduler.Storage.AppointmentStorage.Add(apt1)
            Dim apt2 As Appointment = scheduler.Storage.AppointmentStorage.CreateAppointment(AppointmentType.Normal, Date.Now, Date.Now.AddHours(2))
            apt2.ResourceId = scheduler.Storage.ResourceStorage(1).Id
            scheduler.Storage.AppointmentStorage.Add(apt2)
'            #End Region ' #AllowAppointmentConflictsEvent
        End Sub
        #Region "#@AllowAppointmentConflictsEvent"
        Private Shared Sub Scheduler_AllowAppointmentConflicts(ByVal sender As Object, ByVal e As AppointmentConflictEventArgs)
            e.Conflicts.Clear()
            FillConflictedAppointmentsCollection(e.Conflicts, e.Interval, DirectCast(sender, DevExpress.Xpf.Scheduler.SchedulerControl).Storage.AppointmentStorage.Items, e.AppointmentClone)
        End Sub
        Private Shared Sub FillConflictedAppointmentsCollection(ByVal conflicts As AppointmentBaseCollection, ByVal interval As TimeInterval, ByVal collection As AppointmentBaseCollection, ByVal currApt As Appointment)
            For i As Integer = 0 To collection.Count - 1
                Dim apt As Appointment = collection(i)
                If (New TimeInterval(apt.Start, apt.End)).IntersectsWith(interval) And Not(apt.Start = interval.End OrElse apt.End = interval.Start) Then
                    If apt.ResourceId Is currApt.ResourceId Then
                        conflicts.Add(apt)
                    End If
                End If
                If apt.Type = AppointmentType.Pattern Then
                    FillConflictedAppointmentsCollection(conflicts, interval, apt.GetExceptions(), currApt)
                End If
            Next i
        End Sub
        #End Region ' #@AllowAppointmentConflictsEvent

    End Class
End Namespace
