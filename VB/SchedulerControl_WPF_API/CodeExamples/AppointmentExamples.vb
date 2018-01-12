﻿Imports DevExpress.XtraScheduler
Imports DevExpress.Xpf.Scheduler
Imports System

Namespace SchedulerControl_WPF_API
    Friend Class AppointmentExamples

        Private Shared Sub CreateAppointmentFromSelection(ByVal scheduler As SchedulerControl)
'            #Region "#CreateAppointmentFromSelection"
            ' Delete all appointments.
            scheduler.Storage.AppointmentStorage.Clear()
            ' Select time interval
            scheduler.ActiveView.SetSelection(New TimeInterval(Date.Now, New TimeSpan(2, 40, 0)), scheduler.ActiveView.GetResources()(1))
            ' Group by resource.
            scheduler.GroupType = SchedulerGroupType.Resource
            ' Create a new appointment.
            Dim apt As Appointment = scheduler.Storage.CreateAppointment(AppointmentType.Normal)

            ' Set the appointment's time interval to the selected time interval.
            apt.Start = scheduler.SelectedInterval.Start
            apt.End = scheduler.SelectedInterval.End

            ' Set the appointment's resource to the resource which contains
            ' the currently selected time interval.
            apt.ResourceId = scheduler.SelectedResource.Id

            apt.LabelKey = scheduler.Storage.AppointmentStorage.Labels.GetByIndex(1).Id

            ' Add the new appointment to the appointment collection.
            scheduler.Storage.AppointmentStorage.Add(apt)
'            #End Region ' #CreateAppointmentFromSelection
        End Sub

        Private Shared Sub UseCustomFields(ByVal scheduler As SchedulerControl)
'            #Region "#UseCustomFields"
            scheduler.Storage.AppointmentStorage.Clear()
            ' Handle the FilterAppointment event to display appointments by some criteria.
            AddHandler scheduler.Storage.FilterAppointment, AddressOf Storage_FilterAppointment
            scheduler.DayView.TopRowTime = TimeSpan.FromHours(Date.Now.Hour)

            ' Add mapping to create a custom field. 
            ' If Scheduler is bound to data, the string "DataFieldOne" should match the field name in the data source.
            ' For unbound Scheduler, the data field name is required but not used.
            scheduler.Storage.AppointmentStorage.CustomFieldMappings.Add(New SchedulerCustomFieldMapping("PropertyOne", "DataFieldOne"))
            Dim apt1 As Appointment = scheduler.Storage.CreateAppointment(AppointmentType.Normal, Date.Now, New TimeSpan(1, 0, 0), "Visible Appointment")
            apt1.CustomFields("PropertyOne") = "Visible"
            scheduler.Storage.AppointmentStorage.Add(apt1)
            Dim apt2 As Appointment = scheduler.Storage.CreateAppointment(AppointmentType.Normal, Date.Now.AddHours(2), New TimeSpan(0, 30, 0), "Hidden Appointment")
            apt2.CustomFields("PropertyOne") = "Hidden"
            scheduler.Storage.AppointmentStorage.Add(apt2)
'            #End Region ' #UseCustomFields
        End Sub

        #Region "#@UseCustomFields"
        Private Shared Sub Storage_FilterAppointment(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)
            If CType(e.Object, Appointment).CustomFields("PropertyOne") IsNot Nothing Then
                e.Cancel = CType(e.Object, Appointment).CustomFields("PropertyOne").ToString() = "Hidden"
            End If
        End Sub
        #End Region ' #@UseCustomFields
    End Class

End Namespace
