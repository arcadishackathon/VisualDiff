﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;

using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph;
using Dynamo.Models;
using Dynamo.UI.Commands;

namespace Track.src
{
    class Track_Functions
    {
        //Fields
        private bool ReferenceFileExists = false;

        public bool CheckReferenceDynamoGraphFileLocationValidity(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return ReferenceFileExists = true;
            }
            else
            {
                return ReferenceFileExists = false;
            }
        }

        public void CompareSomeGraphs(ViewLoadedParams viewLoadedParams, string FilePath)
        {
            //Add Laurence's stuff here to generate the lists


            var v1 = viewLoadedParams.CurrentWorkspaceModel.Nodes;

            DynamoViewModel viewModel = viewLoadedParams.DynamoWindow.DataContext as DynamoViewModel;

            // DynamoViewModel dynamoViewModel => viewLoadedParams.DynamoWindow.DataContext as DynamoViewModel;

            string referenceFile = FilePath; // "C:\\Users\\elsdonl0213\\Repos\\Collaborate\\Resources\\example-v2.dyn";

            var graphCount = 0;
            // Cycle through all files found in the directory
            var ext = System.IO.Path.GetExtension(referenceFile);
            // We're only interested in *.dyn files
            if (ext == ".dyn")
            {
                // Open the graph
                viewModel.OpenCommand.Execute(referenceFile);
                // Set the graph run type to manual mode (otherwise some graphs might auto-execute at this point)
                viewModel.CurrentSpaceViewModel.RunSettingsViewModel.Model.RunType = RunType.Manual;
                // Call our main method
                //UnfancifyGraph();
                // Save the graph
                //viewModel.SaveAsCommand.Execute(graph);
                // Close it
                //viewModel.CloseHomeWorkspaceCommand.Execute(null);
                // Increment our counter
                graphCount += 1;
                // Update the message in the UI
                //UnfancifyMsg += "Unfancified " + graph + "\n";
            }

            var v2 = viewLoadedParams.CurrentWorkspaceModel.Nodes;

            // v1
            //Console.WriteLine(v1.CurrentWorkspaceModel.Nodes);
            Console.WriteLine(v1);

            // v2
            Console.WriteLine(v2);

            // It could be this simple!
            /*var added = v1.Except(v2);
            var deleted = v2.Except(v1);*/

            //v1.Nodes

            /*var added = new List<string>();
            var deleted = new List<string>();
            var modified = new List<string>();*/

            var v1Dict = new Dictionary<string, Object>();
            var v2Dict = new Dictionary<string, Object>();

            // Dictionary of V1
            foreach (var node1 in v1)
            {
                v1Dict.Add(node1.GUID.ToString(), node1);
            }
             
            // Dictionary of V2
            foreach (var node2 in v2)
            {
                v2Dict.Add(node2.GUID.ToString(), node2);
            }


            // Why Doesn't this work?
            IEnumerable<string> deleted = v1Dict.Keys.Except(v2Dict.Keys);
            IEnumerable<string> added = v2Dict.Keys.Except(v1Dict.Keys);
            IEnumerable<string> remaining = v2Dict.Keys.Except(added.ToList());

            //Console.WriteLine(deleted.ToList());
            //Console.WriteLine(added.ToList());


            Debug.WriteLine(string.Join("\n\n", new string[] {
                "",
                "These were added:",
                String.Join(", ", added.ToList()),
                "These were deleted:",
                String.Join(", ", deleted.ToList()),
                "These are the remaining:",
                String.Join(", ", remaining.ToList()),
                ""
            }));

            foreach (var key in added)
            {

                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var node = v2Dict[key];

                //keep the data to another method
                ToggleAddedNodes();
            }


        }
        public void ToggleRemovedNodes()
        {
            //Code for comparing added nodes here

            // 1) First add the node on the graph


        }


        public void ToggleAddedNodes()
        {
            //Code for comparing added nodes here

            // 1) First add the node on the graph


        }

    }
}
