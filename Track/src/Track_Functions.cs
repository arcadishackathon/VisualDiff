using System;
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
using Dynamo.Controls;
using Xceed.Wpf.AvalonDock.Controls;


namespace Track.src
{
    class Track_Functions
    {
        //Fields
        Dictionary<string, NodeModel> AddedNodesDictionary = new Dictionary<string, NodeModel>();
        Dictionary<string, NodeModel> DeletedNodesDictionary = new Dictionary<string, NodeModel>();
        Dictionary<string, ConnectorModel> AddedConnectorsDictionary = new Dictionary<string, ConnectorModel>();
        Dictionary<string, ConnectorModel> DeletedConnectorsDictionary = new Dictionary<string, ConnectorModel>();


        int delay = 200;

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

            //Get the data from the currently opened Dynamo graph, nodes and connectors
            var CurrentDynamoGraphNodes = viewLoadedParams.CurrentWorkspaceModel.Nodes;
            var CurrentDynamoGraphConnectors = viewLoadedParams.CurrentWorkspaceModel.Connectors;

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
            var ReferenceDynamoGraphNodes = viewLoadedParams.CurrentWorkspaceModel.Nodes;
            var ReferenceDynamoGraphConnectors = viewLoadedParams.CurrentWorkspaceModel.Connectors;

            // Reset the loaded file by reopening the initially opened (current) graph
            viewModelField.OpenCommand.Execute(CurrentDynamoGraphFileName);

            // -----> do stuff with nodes <----- 

            //create dictionaries containing the nodes
            var currentNodeDict = new Dictionary<string, NodeModel>();
            var referenceNodeDict = new Dictionary<string, NodeModel>();

            // Dictionary of current nodes
            foreach (var node1 in CurrentDynamoGraphNodes)
                currentNodeDict.Add(node1.GUID.ToString(), node1);
             
            // Dictionary of reference nodes
            foreach (var node2 in ReferenceDynamoGraphNodes)
                referenceNodeDict.Add(node2.GUID.ToString(), node2);

            // Create the difference sets of the nodes
            IEnumerable<string> addedNodes = currentNodeDict.Keys.Except(referenceNodeDict.Keys);
            IEnumerable<string> deletedNodes = referenceNodeDict.Keys.Except(currentNodeDict.Keys);
            IEnumerable<string> remainingNodes = currentNodeDict.Keys.Except(addedNodes.ToList());

            //Put the __added__ nodes list in the private field list
            foreach (var key in addedNodes)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var node = currentNodeDict[key];
                AddedNodesDictionary.Add(key, node);
            }

            //Put the __deleted__ nodes list in the private field list
            foreach (var key in deletedNodes)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var node = referenceNodeDict[key];
                DeletedNodesDictionary.Add(key, node);
            }


            // -----> do stuff with wires (connectors) <----- 
            //create dictionaries containing the nodes
            var currentConnectorDict = new Dictionary<string, ConnectorModel>();
            var referenceConnectorDict = new Dictionary<string, ConnectorModel>();

            // Dictionary of current connectors
            foreach (var CurrentConnector in CurrentDynamoGraphConnectors)
                currentConnectorDict.Add(CurrentConnector.GUID.ToString(), CurrentConnector);

            // Dictionary of reference connectors
            foreach (var ReferenceConnector in ReferenceDynamoGraphConnectors)
                referenceConnectorDict.Add(ReferenceConnector.GUID.ToString(), ReferenceConnector);

            // Create the difference sets of the connectors
            IEnumerable<string> addedConnectors = currentConnectorDict.Keys.Except(referenceConnectorDict.Keys);
            IEnumerable<string> deletedConnectors = referenceConnectorDict.Keys.Except(currentConnectorDict.Keys);
            IEnumerable<string> remainingConnectors = currentConnectorDict.Keys.Except(addedConnectors.ToList());

            //Put the __added__ connectors list in the private field list
            foreach (var key in addedConnectors)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var connector = currentConnectorDict[key];
                AddedConnectorsDictionary.Add(key, connector);
            }

            //Put the __deleted__ connectors list in the private field list
            foreach (var key in deletedConnectors)
            {
                // Do stuff with all added nodes
                // Then do the same with all removed/modified etc.
                var connector = referenceConnectorDict[key];
                DeletedConnectorsDictionary.Add(key, connector);
            }








            //ToggleRemovedNodes();

            //test zone



        }



        //Two ideas for the code below
        // 1) should I put all this code in one function and just fill another list with the nodes to handle? 
        //    that would save some extra lines of code -- not right now
        // 2) wiring is deleted when hiding the added nodes. I will have to make sure that is put back on the graph


        public async void FadeNodes()
        {

            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in _nodeViews)
            {

                ((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(50),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

                ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(50),
                    Convert.ToByte(94),
                    Convert.ToByte(92),
                    Convert.ToByte(90)
                ));

                ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(50),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

                ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(50),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

            }
        }
        public async void ColourNodes(Color colour, Dictionary<string, NodeModel> nodes)
        {

            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();
            
            //var nodeView = _nodeViews.First(n => n.ViewModel.Id.ToString().Equals(node.GUID.ToString()));

            // Colour each node
            foreach (var n in _nodeViews)
            {
                //Color brush = Colors.Gray;
                if (nodes.ContainsKey(n.ViewModel.Id.ToString()))
                {
                    //brush = colour;
                    //SolidColorBrush brush = colour;
                    ((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(colour);
                }
                else {
                    // Else style as default
                    // @todo style by STATE
                    /*((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));
                    ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(94),
                        Convert.ToByte(92),
                        Convert.ToByte(90)
                    ));
                    ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));
                    ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));*/
                }


            }
        }

        public async void ColourNodesGrey(Color colour, Dictionary<string, NodeModel> nodes)
        {

            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in _nodeViews)
            {
                //Color brush = Colors.Gray;
                if (nodes.ContainsKey(n.ViewModel.Id.ToString()))
                {
                    ((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));
                    ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(94),
                        Convert.ToByte(92),
                        Convert.ToByte(90)
                    ));
                    ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));
                    ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(Color.FromArgb(
                        Convert.ToByte(255),
                        Convert.ToByte(203),
                        Convert.ToByte(198),
                        Convert.ToByte(190)
                    ));
                }

            }
        }



        public async void UnFadeNodes()
        {
            await Task.Delay(500);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in _nodeViews)
            {
                ((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(255),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

                ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(255),
                    Convert.ToByte(94),
                    Convert.ToByte(92),
                    Convert.ToByte(90)
                ));

                ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(255),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

                ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(255),
                    Convert.ToByte(203),
                    Convert.ToByte(198),
                    Convert.ToByte(190)
                ));

            }
        }


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

                //create the connectors
                foreach (var connector in DeletedConnectorsDictionary)
                {
                    var startGUID = connector.Value.Start.Owner.GUID;
                    var startPortIndex = connector.Value.Start.Index;
                    var startPortType = connector.Value.Start.PortType;
                    var startMode = MakeConnectionCommand.Mode.Begin; 

                    var endGUID = connector.Value.End.Owner.GUID;
                    var endPortIndex = connector.Value.End.Index;
                    var endPortType = connector.Value.End.PortType;
                    var endMode = MakeConnectionCommand.Mode.End; 

                    ////start
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        startGUID, startPortIndex, startPortType, startMode), "", "");
                    ////end
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        endGUID, endPortIndex, endPortType, endMode), "", "");
                }


                //}
                ColourNodes(Colors.Red, DeletedNodesDictionary);
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
                //UnFadeNodes();
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
                //create the connectors
                foreach (var connector in AddedConnectorsDictionary)
                {
                    var startGUID = connector.Value.Start.Owner.GUID;
                    var startPortIndex = connector.Value.Start.Index;
                    var startPortType = connector.Value.Start.PortType;
                    var startMode = MakeConnectionCommand.Mode.Begin;

                    var endGUID = connector.Value.End.Owner.GUID;
                    var endPortIndex = connector.Value.End.Index;
                    var endPortType = connector.Value.End.PortType;
                    var endMode = MakeConnectionCommand.Mode.End;

                    ////start
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        startGUID, startPortIndex, startPortType, startMode), "", "");
                    ////end
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        endGUID, endPortIndex, endPortType, endMode), "", "");
                }
                //colour the node
                //put Rob&Laurence's code here
                ColourNodes(Colors.Green, AddedNodesDictionary);
            }
            if (IsChecked == false) 
            {
                //delete the node
                /*foreach (var node in AddedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new DeleteModelCommand(node.Value.GUID), "", "");
                }*/

                //colour the node
                //put Rob&Laurence's code here

                //UnFadeNodes();

                ColourNodesGrey(Colors.Gray, AddedNodesDictionary);
            }
        }

    }
}
