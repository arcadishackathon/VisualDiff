using System;
using System.IO;
using Dynamo.Wpf.Extensions;
using Dynamo.Graph.Workspaces;
using Dynamo.ViewModels;

namespace Track
{
    public static class Utilies
    {
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
            // Is is it a .dyn file?
            else if (Path.GetExtension(filePath) != ".dyn")
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
    }
}
