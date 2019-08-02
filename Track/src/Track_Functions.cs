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
using System.Windows.Controls;

using Dynamo.Views;

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


        public SolidColorBrush RGBA(int r, int g, int b, int a = 255) {
            return new SolidColorBrush(Color.FromArgb(
                Convert.ToByte(a),
                Convert.ToByte(r),
                Convert.ToByte(g),
                Convert.ToByte(b)
            ));
        }

        public void ColourFill(NodeView n, string name, SolidColorBrush brush)
        {
            ((Rectangle)n.grid.FindName(name)).Fill = brush;
        }

        public void ColourStroke(NodeView n, string name, SolidColorBrush brush)
        {
            ((Rectangle)n.grid.FindName(name)).Stroke = brush;
        }

        public async void FadeNodes()
        {
            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in _nodeViews)
            {
                //ColourFill(n, "nodeBackground", RGBA(203, 198, 190, 50));
                //ColourFill(n, "NameBackground", RGBA(94, 92, 90, 50));
                //ColourStroke(n, "nodeBorder", RGBA(203, 198, 190, 50));
                //ColourStroke(n, "NameBackground", RGBA(203, 198, 190, 50));

                Style style = new Style(n);

                style.nodeBackground(0, 0, 190, 50);
                style.nameBackground(0, 0, 90, 50);
                style.nodeBorder(0, 0, 190, 50);
                style.nameBorder(0, 0, 190, 50);

            }
        }

        public async void ColourNodes(Dictionary<string, NodeModel> nodes, int r, int g, int b, int a = 255)
        {
            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();


            // Colour each node
            foreach (var n in _nodeViews)
            {

                //n.inputcontrol

                /*if (ports.Count() > 0)
                {
                    //ColourFill(n, "highlightOverlay", RGBA(255, 0, 0, 38));
                    //brush = RGBA(255, 0, 0, 38)
                    ((Rectangle)n.inputGrid.FindName("highlightOverlay")).Fill = RGBA(255, 0, 0, 38);

                }*/

                
                if (nodes.ContainsKey(n.ViewModel.Id.ToString()))
                {
                    /*ItemsControl ports = (ItemsControl)n.grid.FindName("inputPortControl");

                    if (ports.Items.Count > 0)
                    {
                        var rects = ports.FindVisualChildren<Rectangle>().ToList();
                        foreach (Rectangle r in rects)
                        {
                            ((Rectangle)r.FindName("highlightOverlay")).Fill = RGBA(255, 255, 0, 255);
                            ((Rectangle)r.FindName("highlightOverlayForArrow")).Fill = RGBA(255, 255, 0, 255);
                        }

                    }*/

                    //ColourFill(n, "nodeBackground", brush);
                    //ColourFill(n, "NameBackground", RGBA(0, 0, 0, 38));
                    //ColourStroke(n, "nodeBorder", brush);
                    //ColourStroke(n, "NameBackground", RGBA(0, 0, 0, 38));

                    Style style = new Style(n);

                    style.nodeBackground(r, g, b, a);
                    style.nodeBorder(r, g, b, a);
                    style.nameBackground(0, 0, 0, 38);
                    style.nameBorder(0, 0, 0, 38);
                    style.portBackground(0, 0, 0, 38);
                }
            }
        }

        public async void ColourNodesGrey(Dictionary<string, NodeModel> nodes)
        {
            await Task.Delay(delay);

            List<NodeView> _nodeViews = ViewLoadedParamsField.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in _nodeViews)
            {
                if (nodes.ContainsKey(n.ViewModel.Id.ToString()))
                {
                    ColourFill(n, "nodeBackground", RGBA(203, 198, 190));
                    ColourFill(n, "NameBackground", RGBA(94, 92, 90));
                    ColourStroke(n, "nodeBorder", RGBA(203, 198, 190));
                    ColourStroke(n, "NameBackground", RGBA(203, 198, 190));
                }
            }
        }

        public void ToggleRemovedNodes(bool IsChecked)
        {
            if (IsChecked)
            {
                // Create the nodes
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                }

                // Create the connectors
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
                ColourNodes(DeletedNodesDictionary, 255, 65, 54);
            }
            else
            {
                // Delete the node
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new DeleteModelCommand(node.Value.GUID), "", "");
                }
            }
        }
        public void ToggleAddedNodes(bool IsChecked)
        {
            if (IsChecked) 
            {
                // Create the nodes
                foreach (var node in AddedNodesDictionary)
                {
                    ViewLoadedParamsField.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                    //will the ModelBase.X actually become obsolete? If this happens, ask Michael Kirschner
                }
                // Create the connectors
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
                ColourNodes(AddedNodesDictionary, 46, 204, 64);
            }
            else
            {
                ColourNodesGrey(AddedNodesDictionary);
            }
        }

    }
}
