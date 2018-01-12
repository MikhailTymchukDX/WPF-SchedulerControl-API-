Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Windows
Imports System.Windows.Documents

Namespace SchedulerControl_WPF_API
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Partial Public Class MainWindow
        Inherits Window

        Private codeEditor As ExampleCodeEditor
        Private evaluator As ExampleEvaluatorByTimer
        Private projectLanguage As ExampleLanguage
        Private richEditControlVBLoaded As Boolean = False
        Private richEditControlCsLoaded As Boolean = False
        'CultureInfo defaultCulture = new CultureInfo("en-US");

        Public Sub New()
            InitializeComponent()
            Dim examplePath As String = "CodeExamples"
            Dim examplesCS As Dictionary(Of String, FileInfo) = CodeExampleDemoUtils.GatherExamplesFromProject(examplePath, ExampleLanguage.Csharp)
            Dim examplesVB As Dictionary(Of String, FileInfo) = CodeExampleDemoUtils.GatherExamplesFromProject(examplePath, ExampleLanguage.VB)
            DisableTabs(examplesCS.Count, examplesVB.Count)
            Dim examples As List(Of CodeExampleGroup) = CodeExampleDemoUtils.FindExamples(examplePath, examplesCS, examplesVB)
            ShowExamplesInTreeList(treeList1, examples)

            AddHandler richEditControlCS.Loaded, AddressOf richEditControlCS_Loaded
            AddHandler richEditControlVB.Loaded, AddressOf richEditControlVB_Loaded

            Me.evaluator = New RichEditExampleEvaluatorByTimer()

            AddHandler Me.evaluator.QueryEvaluate, AddressOf OnExampleEvaluatorQueryEvaluate
            AddHandler Me.evaluator.OnBeforeCompile, AddressOf evaluator_OnBeforeCompile
            AddHandler Me.evaluator.OnAfterCompile, AddressOf evaluator_OnAfterCompile
        End Sub


        Private Sub evaluator_OnAfterCompile(ByVal sender As Object, ByVal args As OnAfterCompileEventArgs)
            If codeEditor IsNot Nothing Then
                codeEditor.AfterCompile(args.Result)
            End If
        End Sub

        Private Sub evaluator_OnBeforeCompile(ByVal sender As Object, ByVal args As EventArgs)
            codeEditor.BeforeCompile()
        End Sub

        Private Sub richEditControlCS_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            If richEditControlVBLoaded AndAlso (Not richEditControlCsLoaded) Then
                CreateCodeEditor()
            End If
            richEditControlCsLoaded = True
        End Sub

        Private Sub richEditControlVB_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
            If richEditControlCsLoaded AndAlso (Not richEditControlVBLoaded) Then
                CreateCodeEditor()
            End If
            richEditControlVBLoaded = True
        End Sub

        Private Sub DisableTabs(ByVal examplesCSCount As Integer, ByVal examplesVBCount As Integer)
            If examplesCSCount = 0 Then
                For Each t As DXTabItem In tabControl.Items
                    If t.Header.ToString().StartsWith("CS") Then
                        t.IsEnabled = False
                    End If
                Next t
            End If
            If examplesVBCount = 0 Then
                For Each t As DXTabItem In tabControl.Items
                    If t.Header.ToString().StartsWith("VB") Then
                        t.IsEnabled = False
                    End If
                Next t
            End If
        End Sub

        Private Sub CreateCodeEditor()
            System.Diagnostics.Debug.Assert(codeEditor Is Nothing)
            Me.codeEditor = New ExampleCodeEditor(richEditControlCS, richEditControlVB, richEditControlCSClass, richEditControlVBClass)
            Me.codeEditor.CurrentExampleLanguage = CurrentExampleLanguage
            ShowFirstExample()
        End Sub

        Private Sub ShowExamplesInTreeList(ByVal treeList1 As TreeListControl, ByVal examples As List(Of CodeExampleGroup))
            treeList1.ItemsSource = examples
        End Sub

        Private Sub ShowFirstExample()
            projectLanguage = CodeExampleDemoUtils.DetectExampleLanguage("SchedulerControl_WPF_API")
            If projectLanguage = ExampleLanguage.Csharp Then
                Me.tabControl.SelectedIndex = 0
            Else
                Me.tabControl.SelectedIndex = 1
            End If

            treeList1.View.ExpandAllNodes()

            If treeList1.View.Nodes.Count > 0 Then
                treeList1.View.FocusedNode = treeList1.View.Nodes(0).Nodes.First()
            End If
        End Sub

        Public Property CurrentExampleLanguage() As ExampleLanguage
            Get
                If tabControl.SelectedContainer.Header.ToString().StartsWith("CS") Then
                    Return ExampleLanguage.Csharp
                Else
                    Return ExampleLanguage.VB
                End If
            End Get
            Set(ByVal value As ExampleLanguage)
                If Me.codeEditor IsNot Nothing Then
                    Me.codeEditor.CurrentExampleLanguage = value
                End If
            End Set
        End Property
        Private Sub OnNewExampleSelected(ByVal sender As Object, ByVal e As CurrentItemChangedEventArgs)
            Dim newExample As CodeExample = TryCast(e.NewItem, CodeExample)
            Dim oldExample As CodeExample = TryCast(e.OldItem, CodeExample)

            If newExample Is Nothing Then
                Return
            End If

            If codeEditor Is Nothing Then
                Return
            End If

            Dim exampleCode As String = codeEditor.ShowExample(oldExample, newExample)
            codeExampleNameLbl.Content = CodeExampleDemoUtils.ConvertStringToMoreHumanReadableForm(newExample.RegionName) & " example"
            Dim args As New CodeEvaluationEventArgs()
            InitializeCodeEvaluationEventArgs(args)
            evaluator.ForceCompile(args)
        End Sub
        Private Sub InitializeCodeEvaluationEventArgs(ByVal e As CodeEvaluationEventArgs)
            e.Result = True
            If codeEditor Is Nothing Then
                Return
            End If

            e.Code = codeEditor.CurrentCodeEditor.Text
            e.CodeClasses = codeEditor.CurrentCodeClassEditor.Text
            e.Language = CurrentExampleLanguage
            displayResultControl.Reset()
            e.EvaluationParameter = displayResultControl.SchedulerControl
        End Sub
        Private Sub OnExampleEvaluatorQueryEvaluate(ByVal sender As Object, ByVal e As CodeEvaluationEventArgs)
            e.Result = False
            If (codeEditor IsNot Nothing) AndAlso codeEditor.RichEditTextChanged Then
                Dim span As TimeSpan = Date.Now.Subtract(codeEditor.LastExampleCodeModifiedTime)

                If span < TimeSpan.FromMilliseconds(1000) Then
                    codeEditor.ResetLastExampleModifiedTime()
                    Return
                End If
                InitializeCodeEvaluationEventArgs(e)
            End If
        End Sub

        Private Sub tabControl_SelectionChanged(ByVal sender As Object, ByVal e As DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs)
            CurrentExampleLanguage = If(CType(e.NewSelectedItem, DXTabItem).Header.ToString().StartsWith("CS"), ExampleLanguage.Csharp, ExampleLanguage.VB)
        End Sub

        Private Sub view_CustomColumnDisplayText(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.TreeList.TreeListCustomColumnDisplayTextEventArgs)

            If e.Node.HasChildren AndAlso TypeOf e.Node.Content Is CodeExampleGroup Then
                e.DisplayText = (TryCast(e.Node.Content, CodeExampleGroup)).Name
            End If
        End Sub
    End Class
End Namespace
