Imports DevExpress.Xpf.Scheduler
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Namespace SchedulerControl_WPF_API
    ''' <summary>
    ''' Interaction logic for DisplayResultControl.xaml
    ''' </summary>
    Partial Public Class DisplayResultControl
        Inherits UserControl

        Private CustomResourceCollection As New BindingList(Of CustomResource)()
        Private CustomEventList As New BindingList(Of CustomAppointment)()

        Public Property SchedulerControl() As SchedulerControl

        Public Sub New()
            InitializeComponent()
            Me.SchedulerControl = Me.scheduler

            InitializeScheduler()
        End Sub

        Private Sub InitializeScheduler()
            InitHelper.InitResources(CustomResourceCollection)
            InitHelper.InitAppointments(CustomEventList, CustomResourceCollection)

            Dim mappingsResource As ResourceMapping = Me.scheduler.Storage.ResourceStorage.Mappings
            mappingsResource.Id = "ResID"
            mappingsResource.Caption = "Name"

            Dim mappingsAppointment As AppointmentMapping = Me.scheduler.Storage.AppointmentStorage.Mappings
            mappingsAppointment.Start = "StartTime"
            mappingsAppointment.End = "EndTime"
            mappingsAppointment.Subject = "Subject"
            mappingsAppointment.AllDay = "AllDay"
            mappingsAppointment.Description = "Description"
            mappingsAppointment.Label = "Label"
            mappingsAppointment.Location = "Location"
            mappingsAppointment.RecurrenceInfo = "RecurrenceInfo"
            mappingsAppointment.ReminderInfo = "ReminderInfo"
            mappingsAppointment.ResourceId = "OwnerId"
            mappingsAppointment.Status = "Status"
            mappingsAppointment.Type = "EventType"

            Me.scheduler.Storage.BeginUpdate()
            Me.scheduler.Storage.ResourceStorage.DataSource = CustomResourceCollection
            Me.scheduler.Storage.AppointmentStorage.DataSource = CustomEventList
            Me.scheduler.Storage.EndUpdate()

            Me.scheduler.Start = Date.Now
            Me.scheduler.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Day
        End Sub

        Public Sub Reset()
            ClearScheduler()
            CustomResourceCollection.Clear()
            CustomEventList.Clear()
            InitializeScheduler()
        End Sub

        Private Sub ClearScheduler()
            Me.scheduler.Storage.BeginUpdate()
            Me.scheduler.Storage.ResourceStorage.Mappings = New ResourceMapping()
            Me.scheduler.Storage.AppointmentStorage.Mappings = New AppointmentMapping()
            Me.scheduler.Storage.ResourceStorage.DataSource = Nothing
            Me.scheduler.Storage.AppointmentStorage.DataSource = Nothing
            Me.scheduler.Storage.EndUpdate()
        End Sub
    End Class
End Namespace
