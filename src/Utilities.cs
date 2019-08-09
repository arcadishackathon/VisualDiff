using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dynamo.Wpf.Extensions;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.ViewModels;

namespace Track
{
    public static class Utilities
    {

        public static string[] DynamoFileExtensions = { ".dyn", ".dyf" };

        public static Tuple<bool, string> CheckReferenceFileIsValid(string filePath, ViewLoadedParams vlp)
        {
            // Assume it's true by default
            // We'll change this later if it's not valid
            bool valid = true;
            string message = "File is valid";

            // Load the current workspace
            WorkspaceModel currentWorkspace = vlp.CurrentWorkspaceModel as WorkspaceModel;

            // Does the file exist
            if (!File.Exists(filePath))
            {
                valid = false;
                message = "File not found";
            }
            // Is is it a .dyn or .dyf file?
            else if ( ! DynamoFileExtensions.Contains(Path.GetExtension(filePath)))
            {
                valid = false;
                message = "File is not a Dynamo Graph";
            }
            else if (currentWorkspace.FileName == "")
            {
                valid = false;
                message = "Open a Graph in Dynamo first";
            }
            else if (currentWorkspace.HasUnsavedChanges)
            {
                valid = false;
                message = "Your current graph has unsaved changes. Save them first";
            }
            return Tuple.Create(valid, message);
        }

        public static void LoadGraph(string fileName, DynamoViewModel viewModel, bool loadManual = true)
        {
            // Open the graph
            viewModel.OpenCommand.Execute(fileName);

            // If the file is a .dyn then load it in Manual mode
            if (Path.GetExtension(fileName) == ".dyn" && loadManual)
            {
                // Set the graph run type to manual mode (otherwise some graphs might auto-execute at this point)
                viewModel.CurrentSpaceViewModel.RunSettingsViewModel.Model.RunType = RunType.Manual;
            }
        }

        public static void Debug(Exception e, string eMessage = "", string context = "", string action = "")
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            //Debug.AutoFlush = true;
            //Debug.Indent();
            System.Diagnostics.Debug.WriteLine(string.Join("\n\n", new string[] {
                "",
                $"'{e}'",
                context,
                eMessage,
                action,
                ""
            }));
            //Debug.Unindent();
        }
    }
}
