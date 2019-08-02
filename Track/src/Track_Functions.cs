using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Controls;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using static Dynamo.Models.DynamoModel;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;
using Xceed.Wpf.AvalonDock.Controls;


namespace Track.src
{
    class Track_Functions
    {

        /// <summary>
        /// Dictionary of ADDED nodes retrievable by their GUID string.
        /// </summary>
        Dictionary<string, NodeModel> AddedNodesDictionary = new Dictionary<string, NodeModel>();

        /// <summary>
        /// Dictionary of DELETED nodes retrievable by their GUID string.
        /// </summary>
        Dictionary<string, NodeModel> DeletedNodesDictionary = new Dictionary<string, NodeModel>();

        /// <summary>
        /// Dictionary of ADDED connectors retrievable by their GUID string.
        /// </summary>
        Dictionary<string, ConnectorModel> AddedConnectorsDictionary = new Dictionary<string, ConnectorModel>();

        /// <summary>
        /// Dictionary of DELETED connectors retrievable by their GUID string.
        /// </summary>
        Dictionary<string, ConnectorModel> DeletedConnectorsDictionary = new Dictionary<string, ConnectorModel>();

        /// <summary>
        /// The delay in ms before attempting to locate a node NodeView
        /// </summary>
        private int _delay = 200;

        /// <summary>
        /// Reference to the current Dynamo DynamoViewModel
        /// </summary>
        DynamoViewModel ViewModel;

        /// <summary>
        /// Reference to the current Dynamo ViewLoadedParams
        /// </summary>
        ViewLoadedParams ViewLoadedParams;

        //Methods
        public bool CheckReferenceFileIsValid(string FilePath)
        {
            if (File.Exists(FilePath))
                return true;
            else
                return false;
        }

        public void CompareSomeGraphs(ViewLoadedParams vlp, string ReferenceGraphFileName)
        {

            //Store the ViewModel and ViewLoadedParams as fields so they can be used in other methods
            ViewModel = vlp.DynamoWindow.DataContext as DynamoViewModel;
            ViewLoadedParams = vlp;


            //Clear any old stuff that may be present in the dictionaries
            //for now just clear the dictionaries. There should be a better way to do it, 
            //maybe by creating a new instance of this class?
            AddedNodesDictionary.Clear();
            DeletedNodesDictionary.Clear();

            //Get the nodes and connectors from the CURRENT Dynamo Graph
            var CurrentGraphNodes = ViewLoadedParams.CurrentWorkspaceModel.Nodes;
            var CurrentGraphConnectors = ViewLoadedParams.CurrentWorkspaceModel.Connectors;

            //Store the filename of the CURRENT Dynamo Graph
            string CurrentGraphFileName = ViewLoadedParams.CurrentWorkspaceModel.FileName;

            //Check if the ReferenceGraphFileName is actually a Dynamo graph and if so open it
            var ext = System.IO.Path.GetExtension(ReferenceGraphFileName);

            // We're only interested in *.dyn files
            // @todo move this to CheckReferenceFileIsValid()
            if (ext == ".dyn")
            {
                // Open the graph
                ViewModel.OpenCommand.Execute(ReferenceGraphFileName);
                // Set the graph run type to manual mode (otherwise some graphs might auto-execute at this point)
                ViewModel.CurrentSpaceViewModel.RunSettingsViewModel.Model.RunType = RunType.Manual;
            }

            //Get the nodes and connectors from the REFERENCE Dynamo Graph
            var ReferenceGraphNodes = ViewLoadedParams.CurrentWorkspaceModel.Nodes;
            var ReferenceGraphConnectors = ViewLoadedParams.CurrentWorkspaceModel.Connectors;

            // Reset the loaded file by reopening the initially opened (CURRENT) graph
            ViewModel.OpenCommand.Execute(CurrentGraphFileName);

            // -----> do stuff with nodes <-----

            //create dictionaries containing the nodes
            var currentNodeDict = new Dictionary<string, NodeModel>();
            var referenceNodeDict = new Dictionary<string, NodeModel>();

            // Create a dictionary of CURRENT nodes by GUID
            foreach (NodeModel node in CurrentGraphNodes)
            {
                currentNodeDict.Add(node.GUID.ToString(), node);
            }

            // Create a dictionary of REFERENCE nodes by GUID
            foreach (NodeModel node in ReferenceGraphNodes)
            {
                referenceNodeDict.Add(node.GUID.ToString(), node);
            }

            // Calculate the difference between CURRENT and REFERENCE nodes
            IEnumerable<string> addedNodes = currentNodeDict.Keys.Except(referenceNodeDict.Keys);
            IEnumerable<string> deletedNodes = referenceNodeDict.Keys.Except(currentNodeDict.Keys);
            IEnumerable<string> remainingNodes = currentNodeDict.Keys.Except(addedNodes.ToList());

            //Put the ADDED nodes list in the private field list
            foreach (var key in addedNodes)
            {
                NodeModel node = currentNodeDict[key];
                AddedNodesDictionary.Add(key, node);
            }

            //Put the DELETED nodes list in the private field list
            foreach (var key in deletedNodes)
            {
                NodeModel node = referenceNodeDict[key];
                DeletedNodesDictionary.Add(key, node);
            }

            // -----> do stuff with wires (connectors) <-----
            //create dictionaries containing the nodes
            var currentConnectorDict = new Dictionary<string, ConnectorModel>();
            var referenceConnectorDict = new Dictionary<string, ConnectorModel>();

            // Create a dictionary of CURRENT connectors by GUID
            foreach (ConnectorModel connector in CurrentGraphConnectors)
            {
                currentConnectorDict.Add(connector.GUID.ToString(), connector);
            }

            // Create a dictionary of REFERENCE connectors by GUID
            foreach (ConnectorModel connector in ReferenceGraphConnectors)
            {
                referenceConnectorDict.Add(connector.GUID.ToString(), connector);
            }

            // Create the difference sets of the connectors
            IEnumerable<string> addedConnectors = currentConnectorDict.Keys.Except(referenceConnectorDict.Keys);
            IEnumerable<string> deletedConnectors = referenceConnectorDict.Keys.Except(currentConnectorDict.Keys);
            IEnumerable<string> remainingConnectors = currentConnectorDict.Keys.Except(addedConnectors.ToList());

            //Put the ADDED connectors list in the private field list
            foreach (var key in addedConnectors)
            {
                ConnectorModel connector = currentConnectorDict[key];
                AddedConnectorsDictionary.Add(key, connector);
            }

            //Put the DELETED connectors list in the private field list
            foreach (var key in deletedConnectors)
            {
                ConnectorModel connector = referenceConnectorDict[key];
                DeletedConnectorsDictionary.Add(key, connector);
            }

        }

        // @todo 1) should I put all this code in one function and just fill another list with the nodes to handle? that would save some extra lines of code -- not right now
        // @todo 2) wiring is deleted when hiding the added nodes. I will have to make sure that is put back on the graph

        public async void ColourNodes(Color colour, Dictionary<string, NodeModel> nodes)
        {
            await Task.Delay(_delay);

            List<NodeView> nodeViews = ViewLoadedParams.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in nodeViews)
            {
                if (nodes.ContainsKey(n.ViewModel.Id.ToString()))
                {
                    ((Rectangle)n.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("NameBackground")).Fill = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("nodeBorder")).Stroke = new SolidColorBrush(colour);
                    ((Rectangle)n.grid.FindName("NameBackground")).Stroke = new SolidColorBrush(colour);
                }
            }
        }

        public async void ColourNodesGrey(Color colour, Dictionary<string, NodeModel> nodes)
        {
            await Task.Delay(_delay);

            List<NodeView> nodeViews = ViewLoadedParams.DynamoWindow.FindVisualChildren<NodeView>().ToList();

            // Colour each node
            foreach (var n in nodeViews)
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

        public void ToggleDeletedNodes(bool IsChecked)
        {
            // Add the nodes from the REFERENCE graph that were deleted in the CURRENT graph
            // Add the connectors from the REFERENCE graph that were deleted in the CURRENT graph 
            // Colour the DELETED nodes red
            // @todo Colour the deleted connectors
            if (IsChecked)
            {
                // Go through the DELETED nodes and add each one
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                    // @todo will the ModelBase.X actually become obsolete? If this happens, ask Michael Kirschner
                }

                // Go through the DELTED connectors and add each one
                foreach (var connector in DeletedConnectorsDictionary)
                {
                    // First set the START
                    var startGUID = connector.Value.Start.Owner.GUID;
                    var startPortIndex = connector.Value.Start.Index;
                    var startPortType = connector.Value.Start.PortType;
                    var startMode = MakeConnectionCommand.Mode.Begin;

                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        startGUID, startPortIndex, startPortType, startMode), "", "");

                    // Then set the END
                    var endGUID = connector.Value.End.Owner.GUID;
                    var endPortIndex = connector.Value.End.Index;
                    var endPortType = connector.Value.End.PortType;
                    var endMode = MakeConnectionCommand.Mode.End;

                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        endGUID, endPortIndex, endPortType, endMode), "", "");
                }

                ColourNodes(Colors.Red, DeletedNodesDictionary);
            }
            // If not checked then delete the DELETED nodes
            // @todo Their connectors disappear automatically is this best practice?
            else
            {
                // Go through the DELETED nodes and delete each one
                foreach (var node in DeletedNodesDictionary)
                {
                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new DeleteModelCommand(node.Value.GUID), "", "");
                }
            }
        }
        public void ToggleAddedNodes(bool IsChecked)
        {
            // Colour the nodes in the CURRENT graph that are not in the REFERENCE graph
            // @todo Colour the added connectors
            if (IsChecked == true)
            {
                //create the nodes
                foreach (var node in AddedNodesDictionary)
                {

                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new CreateNodeCommand(node.Value,
                        node.Value.X, node.Value.Y, false, false), "", "");
                    //@todo will the ModelBase.X actually become obsolete? If this happens, ask Michael Kirschner
                }
                //create the connectors
                foreach (var connector in AddedConnectorsDictionary)
                {
                    // First set the START
                    var startGUID = connector.Value.Start.Owner.GUID;
                    var startPortIndex = connector.Value.Start.Index;
                    var startPortType = connector.Value.Start.PortType;
                    var startMode = MakeConnectionCommand.Mode.Begin;

                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        startGUID, startPortIndex, startPortType, startMode), "", "");

                    // Then set the END
                    var endGUID = connector.Value.End.Owner.GUID;
                    var endPortIndex = connector.Value.End.Index;
                    var endPortType = connector.Value.End.PortType;
                    var endMode = MakeConnectionCommand.Mode.End;

                    ViewLoadedParams.CommandExecutive.ExecuteCommand(new MakeConnectionCommand(
                        endGUID, endPortIndex, endPortType, endMode), "", "");
                }

                ColourNodes(Colors.Green, AddedNodesDictionary);
            }
            else
            {
                ColourNodesGrey(Colors.Gray, AddedNodesDictionary);
            }
        }

    }
}
