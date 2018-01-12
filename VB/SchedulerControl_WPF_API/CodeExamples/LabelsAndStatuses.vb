Imports DevExpress.Xpf.Scheduler
Imports DevExpress.XtraScheduler
Imports System
Imports System.Windows.Media

Namespace SchedulerControl_WPF_API
    Friend Class LabelsAndStatuses
        Private Shared Sub CustomLabelsAndStatusesAction(ByVal scheduler As SchedulerControl)
'            #Region "#CustomLabelsAndStatuses"
            scheduler.Storage.AppointmentStorage.Clear()

            Dim IssueList() As String = { "Consultation", "Treatment", "X-Ray" }
            Dim IssueColorList() As Color = { Colors.Ivory, Colors.Pink, Colors.Plum }
            Dim PaymentStatuses() As String = { "Paid", "Unpaid" }
            Dim PaymentColorStatuses() As Color = { Colors.Green, Colors.Red }


            Dim labelStorage As IAppointmentLabelStorage = scheduler.Storage.AppointmentStorage.Labels
            labelStorage.Clear()
            Dim count As Integer = IssueList.Length
            For i As Integer = 0 To count - 1
                Dim label As IAppointmentLabel = labelStorage.CreateNewLabel(i, IssueList(i))
                label.SetColor(IssueColorList(i))
                labelStorage.Add(label)
            Next i
            Dim statusStorage As IAppointmentStatusStorage = scheduler.Storage.AppointmentStorage.Statuses
            statusStorage.Clear()
            count = PaymentStatuses.Length
            For i As Integer = 0 To count - 1
                Dim status As IAppointmentStatus = statusStorage.CreateNewStatus(i, PaymentStatuses(i), PaymentStatuses(i))
                status.SetBrush(New SolidColorBrush(PaymentColorStatuses(i)))
                statusStorage.Add(status)
            Next i

            ' Create a new appointment.
            Dim apt As Appointment = scheduler.Storage.CreateAppointment(AppointmentType.Normal)
            apt.Subject = "Test"
            apt.Start = Date.Now
            apt.End = Date.Now.AddHours(2)
            apt.ResourceId = scheduler.Storage.ResourceStorage(0).Id
            apt.LabelKey = labelStorage.GetByIndex(2).Id
            scheduler.Storage.AppointmentStorage.Add(apt)

'            #End Region ' #CustomLabelsAndStatuses
        End Sub
    End Class
End Namespace
