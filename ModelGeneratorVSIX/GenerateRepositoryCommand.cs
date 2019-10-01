using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using ModelGenerator.Enum;
using Task = System.Threading.Tasks.Task;

namespace ModelGenerator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GenerateRepositoryCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;
        public const int CommandId2 = 0x0101;
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("afdef772-1399-4bab-801b-f635202e043e");
        public static readonly Guid CommandSet2 = new Guid("e9cef5d6-fbe3-4435-bf4f-84b6a4534fe4");
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateRepositoryCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GenerateRepositoryCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            var menuCommandID2 = new CommandID(CommandSet2, CommandId2);
            var menuItem2 = new MenuCommand(this.Execute2, menuCommandID2);
            commandService.AddCommand(menuItem);
            commandService.AddCommand(menuItem2);
        }

        private void Execute2(object sender, EventArgs e)
        {
            ExecutionMethod((l, c, d, p) =>
            {
                LangaugesData.PerformModelGenerate(l, TargetDatabaseConnector.SQLServer, c, d, p);
            });
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GenerateRepositoryCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GenerateRepository's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new GenerateRepositoryCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ExecutionMethod((l, c, d, p) =>
            {
                LangaugesData.PerformStrategyGenerate(l, TargetDatabaseConnector.SQLServer, c, d, p);
            });
        }
        private void ExecutionMethod(Action<TargetLanguage, string, string, string> action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProject = GetSelectedProject();
            TargetLanguage targetLanguage = TargetLanguage.CSharp;
            string lang = null;
            try
            {
                lang = selectedProject.CodeModel.Language;
            }
            catch
            {
                
            }
            switch (lang)
            {
                case EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB:
                    targetLanguage = TargetLanguage.VisualBasic;
                    break;
                case EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp:
                    targetLanguage = TargetLanguage.CSharp;
                    break;
                default:
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Unsupported project, make sure that the project is either C# or Visual Basic.",
                        "ModelGenerator",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
            }
            var connectionString = Microsoft.VisualBasic.Interaction.InputBox("Connection string to Microsoft SQL Server");

            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var directory = Path.GetDirectoryName(selectedProject.FullName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                VsShellUtilities.ShowMessageBox(
                    this.package,
                    "Action request cancel by user.",
                    "ModelGenerator",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                return;
            }
            var solutionName = Path.GetFileNameWithoutExtension(dte2.Solution.FullName);
            var projectName = selectedProject.Name;
            var defaultNamespace = selectedProject.Properties.Item("DefaultNamespace").Value.ToString();
            dte2.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Activate();
            dte2.ToolWindows.SolutionExplorer.GetItem($@"{solutionName}\{projectName}").Select(vsUISelectionType.vsUISelectionTypeSelect);
            //dte2.ExecuteCommand("Project.UnloadProject");
            //LangaugesData.PerformStrategyGenerate(TargetLanguage.CSharp, TargetDatabaseConnector.SQLServer, connectionString, directory, projectName);
            action(targetLanguage, connectionString, directory, defaultNamespace);
            //dte2.ExecuteCommand("Project.ReloadProject");
        }
        private Project GetSelectedProject()
        {
            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

            monitorSelection.GetCurrentSelection(out IntPtr hierarchyPointer,
                                                 out uint projectItemId,
                                                 out IVsMultiItemSelect multiItemSelect,
                                                 out IntPtr selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                 hierarchyPointer,
                                                 typeof(IVsHierarchy)) as IVsHierarchy;
            object selectedObject = null;
            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }
            Project selectedProject = selectedObject as Project;
            return selectedProject;
        }
    }
}
