using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace SchedulerControl_WPF_API
    {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ExampleCodeEditor codeEditor;
        ExampleEvaluatorByTimer evaluator;
        ExampleLanguage projectLanguage;
        bool richEditControlVBLoaded = false;
        bool richEditControlCsLoaded = false;
        //CultureInfo defaultCulture = new CultureInfo("en-US");

        public MainWindow() {
            InitializeComponent();
            string examplePath = "CodeExamples";
            Dictionary<string, FileInfo> examplesCS = CodeExampleDemoUtils.GatherExamplesFromProject(examplePath, ExampleLanguage.Csharp);
            Dictionary<string, FileInfo> examplesVB = CodeExampleDemoUtils.GatherExamplesFromProject(examplePath, ExampleLanguage.VB);
            DisableTabs(examplesCS.Count, examplesVB.Count);
            List<CodeExampleGroup> examples = CodeExampleDemoUtils.FindExamples(examplePath, examplesCS, examplesVB);
            ShowExamplesInTreeList(treeList1, examples);

            richEditControlCS.Loaded += richEditControlCS_Loaded;
            richEditControlVB.Loaded += richEditControlVB_Loaded;

            this.evaluator = new RichEditExampleEvaluatorByTimer();

            this.evaluator.QueryEvaluate += OnExampleEvaluatorQueryEvaluate;
            this.evaluator.OnBeforeCompile += evaluator_OnBeforeCompile;
            this.evaluator.OnAfterCompile += evaluator_OnAfterCompile;
        }


        void evaluator_OnAfterCompile(object sender, OnAfterCompileEventArgs args) {
            if (codeEditor != null)
                codeEditor.AfterCompile(args.Result);
        }

        void evaluator_OnBeforeCompile(object sender, EventArgs args) {
            codeEditor.BeforeCompile();
        }

        void richEditControlCS_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            if (richEditControlVBLoaded && !richEditControlCsLoaded)
                CreateCodeEditor();
            richEditControlCsLoaded = true;
        }

        void richEditControlVB_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            if (richEditControlCsLoaded && !richEditControlVBLoaded )
                CreateCodeEditor();
            richEditControlVBLoaded = true;
        }

        void DisableTabs(int examplesCSCount, int examplesVBCount) {
            if (examplesCSCount == 0)
                foreach (DXTabItem t in tabControl.Items) if (t.Header.ToString().StartsWith("CS")) t.IsEnabled = false;
            if (examplesVBCount == 0)
                foreach (DXTabItem t in tabControl.Items) if (t.Header.ToString().StartsWith("VB")) t.IsEnabled = false;
        }

        void CreateCodeEditor(){
            System.Diagnostics.Debug.Assert(codeEditor == null);
            this.codeEditor = new ExampleCodeEditor(richEditControlCS, richEditControlVB, richEditControlCSClass, richEditControlVBClass);
            this.codeEditor.CurrentExampleLanguage = CurrentExampleLanguage;
            ShowFirstExample();
        }

        void ShowExamplesInTreeList(TreeListControl treeList1, List<CodeExampleGroup> examples) {
            treeList1.ItemsSource = examples;
        }
        
        void ShowFirstExample() {
            projectLanguage = CodeExampleDemoUtils.DetectExampleLanguage("SchedulerControl_WPF_API");
            if (projectLanguage == ExampleLanguage.Csharp)
                this.tabControl.SelectedIndex = 0;
            else
                this.tabControl.SelectedIndex = 1;

            treeList1.View.ExpandAllNodes();

            if (treeList1.View.Nodes.Count > 0)
                treeList1.View.FocusedNode = treeList1.View.Nodes[0].Nodes.First();
        }

        public ExampleLanguage CurrentExampleLanguage
        {
            get
            {
                if (tabControl.SelectedContainer.Header.ToString().StartsWith("CS")) return ExampleLanguage.Csharp;
                else return ExampleLanguage.VB;
            }
            set
            {
                if (this.codeEditor != null)
                {
                    this.codeEditor.CurrentExampleLanguage = value;
                }
            }
        }
        private void OnNewExampleSelected(object sender, CurrentItemChangedEventArgs e) {
            CodeExample newExample = e.NewItem as CodeExample;
            CodeExample oldExample = e.OldItem as CodeExample;

            if (newExample == null )
                return;

            if (codeEditor == null)
                return;

            string exampleCode = codeEditor.ShowExample(oldExample, newExample);
            codeExampleNameLbl.Content = CodeExampleDemoUtils.ConvertStringToMoreHumanReadableForm(newExample.RegionName) + " example";
            CodeEvaluationEventArgs args = new CodeEvaluationEventArgs();
            InitializeCodeEvaluationEventArgs(args);
            evaluator.ForceCompile(args);
        }
        void InitializeCodeEvaluationEventArgs(CodeEvaluationEventArgs e) {
            e.Result = true;
            if (codeEditor == null)                return;

            e.Code = codeEditor.CurrentCodeEditor.Text;
            e.CodeClasses = codeEditor.CurrentCodeClassEditor.Text;
            e.Language = CurrentExampleLanguage;
            displayResultControl.Reset();
            e.EvaluationParameter = displayResultControl.SchedulerControl;
        }
        void OnExampleEvaluatorQueryEvaluate(object sender, CodeEvaluationEventArgs e) {
            e.Result = false;
            if ((codeEditor != null) &&codeEditor.RichEditTextChanged) {
                TimeSpan span = DateTime.Now - codeEditor.LastExampleCodeModifiedTime;

                if (span < TimeSpan.FromMilliseconds(1000)) {
                    codeEditor.ResetLastExampleModifiedTime();
                    return;
                }
                InitializeCodeEvaluationEventArgs(e);
            }
        }

        void tabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e) {
            CurrentExampleLanguage = (((DXTabItem) e.NewSelectedItem).Header.ToString().StartsWith("CS")) ? ExampleLanguage.Csharp : ExampleLanguage.VB;
        }

        private void view_CustomColumnDisplayText(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCustomColumnDisplayTextEventArgs e) {

            if (e.Node.HasChildren && e.Node.Content is CodeExampleGroup) {
                e.DisplayText = (e.Node.Content as CodeExampleGroup).Name;
            }
        }
    }
}
