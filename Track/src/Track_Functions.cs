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
using CoreNodeModels;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Nodes;
using static Dynamo.Models.DynamoModel;

namespace Track.src
{
    class Track_Functions
    {
        //Fields
        Dictionary<string, NodeModel> AddedNodesDictionary = new Dictionary<string, NodeModel>();
        Dictionary<string, NodeModel> DeletedNodesDictionary = new Dictionary<string, NodeModel>();

        DynamoViewModel viewModelField;
        ViewLoadedParams ViewLoadedParamsField;

        //Methods
        public bool CheckReferenceDynamoGraphFileLocationValidity(string FilePath)
        {
            if (File.Exists(FilePath))
                return true;
            else
                return false;
        }


        public void CompareSomeGraphs(ViewLoadedParams viewLoadedParams, string FilePath)
        {
            //first clear any old stuff that may be present in the dictionaries
            //for now just clear the dictionaries. There should be a better way to do it, 
            //maybe by creating a new instance of this class?
            AddedNodesDictionary.Clear();
            DeletedNodesDictionary.Clear();


            //Get the data from the currently opened Dynamo graph
            var CurrentDynamoGraph = viewLoadedParams.CurrentWorkspaceModel.Nodes;

            //store the filename so it can be reopened later
            string CurrentDynamoGraphFileName = viewLoadedParams.CurrentWorkspaceModel.FileName;

            //Store the viewModel and ViewLoadedParams als fields so they can be used in other methods
            viewModelField = viewLoadedParams.DynamoWindow.DataContext as DynamoViewModel;
            ViewLoadedParamsField = viewLoadedParams;

            //Check if the opened file is actually a Dynamo graph and if so open it
            var ext = System.IO.Path.GetExtension(FilePath);
            // We're only interested in *.dyn files
            if (ext == ".dyn")
            {
                // Open the graph
                viewModelField.OpenCommand.Execute(FilePath);
                // Set the graph run type to manual mode (otherwise some graphs might auto-execute at this point)
                viewModelField.CurrentSpaceViewModel.RunSettingsViewModel.Model.RunType = RunType.Manual;
                // Call our main method
                //UnfancifyGraph();
                // Save the graph
                //viewModel.SaveAsCommand.Execute(graph);
                // Close it
                //viewModel.CloseHomeWorkspaceCommand.Execute(null);
                // Increment our counter
                //graphCount += 1;
                // Update the message in the UI
                //UnfancifyMsg += "Unfancified " + graph + "\n";
            }


            // Read the reference graph
            var ReferenceDynamoGraph = viewLoadedParams.CurrentWorkspaceModel.Nodes;

            // Reset the loaded file by reopening the initially opened (current) graph
            viewModelField.OpenCommand.Execute(CurrentDynamoGraphFileName);

            // v1
            //Console.WriteLine(v1.CurrentWorkspaceModel.Nodes);
            Console.WriteLine(CurrentDynamoGraph);

            // v2
            Console.WriteLine(ReferenceDynamoGraph);

            // It could be this simple!
            /*var added = v1.Except(v2);
            var deleted = v2.Except(v1);*/

            //v1.Nodes

            /*var added = new List<string>();
            var deleted = new List<string>();
            var modified = new List<string>();*/

            //create dictionaries containing the nodes
            var currentNodeDict = new Dictionary<string, NodeModel>();
            var referenceNodeDict = new Dictionary<string, NodeModel>();

            // Dictionary of V1
            foreach (var node1 in CurrentDynamoGraph)
            {
                currentNodeDict.Add(node1.GUID.ToString(), node1);
            }
             
            // Dictionary of V2
            foreach (var node2 in ReferenceDynamoGraph)
            {
                referenceNodeDict.Add(node2.GUID.ToString(), node2);
            }


            // Why Doesn't this work? It does work ;)
            IEnumerable<string> added = currentNodeDict.Keys.Except(referenceNodeDict.Keys);
            IEnumerable<string> deleted = referenceNodeDict.Keys.Except(currentNodeDict.Keys);
            IEnumerable<string> remaining = currentNodeDict.Keys.Except(added.ToList());


            //Console.WriteLine(deleted.ToList());
            //Console.WriteLine(added.ToList());


            Debug.WriteLine(string.Join("\n\n", new string[] {
                "",
                "These were added:",
                string.Join(", ", added.ToList()),
                "These were deleted:",
                string.Join(", ", deleted.ToList()),
                "These are the remaining:",
                string.Join(", ", remaining.ToList()),
                ""
            }));

            //Put the __added__ nodes list in the private field list
            foreach (var key in added)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var node = currentNodeDict[key];
                AddedNodesDictionary.Add(key, node);
            }

            //Put the __deleted__ nodes list in the private field list
            foreach (var key in deleted)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var node = referenceNodeDict[key];
                DeletedNodesDictionary.Add(key, node);
            }
            //ToggleRemovedNodes();

            //test zone



        }



        //Two ideas for the code below
        // 1) should I put all this code in one function and just fill another list with the nodes to handle? 
        //    that would save some extra lines of code
        // 2) wiring is deleted when showing the added nodes. I will have to make sure that is put back on the graph





        public void ToggleRemovedNodes(bool IsChecked)
        {
            //Code for comparing added nodes here
            //DeletedNodesDictionary
            // 1) First add the node on the graph
            // 2) Colour the node on the graph
            // 3) When fed up with looking at it, remove the node upon toggle disable or closing the viewextention

            //add the node on the graph
            if (IsChecked)
            {
                //create the nodes
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                    //will the ModelBase.X actually become obsolete? If this happens, ask Michael Kirschner
                }

                //colour the node
                //put Rob&Laurence's code here
            }

            //Remove the node from the graph
            if (IsChecked == false)
            {
                //delete the node
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new DeleteModelCommand(node.Value.GUID), "", "");
                }

                //colour the node
                //put Rob&Laurence's code here
            }
        }
        public void ToggleAddedNodes(bool IsChecked)
        {
            //Code for comparing added nodes here
            //DeletedNodesDictionary
            // 1) First add the node on the graph
            // 2) Colour the node on the graph
            // 3) When fed up with looking at it, remove the node upon toggle disable or closing the viewextention

            //add the node on the graph
            if (IsChecked) 
            {
                //create the nodes
                foreach (var node in AddedNodesDictionary)
                {

                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                    //will the ModelBase.X actually become obsolete? If this happens, ask Michael Kirschner
                }

                //colour the node
                //put Rob&Laurence's code here
            }
            if (IsChecked == false) 
            {
                //delete the node
                foreach (var node in AddedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new DeleteModelCommand(node.Value.GUID), "", "");
                }

                //colour the node
                //put Rob&Laurence's code here
            }
        }

    }
}
