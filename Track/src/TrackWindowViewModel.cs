using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;

using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Graph;
using Dynamo.Models;
using Dynamo.UI.Commands;
using Dynamo.Wpf;
using Dynamo.Controls;
using Xceed.Wpf.AvalonDock.Controls;

namespace Track
{
    public class TrackWindowViewModel : NotificationObject, IDisposable
    {

        public static List<NodeView> breakTheWheel;

        public TrackWindowViewModel(ViewLoadedParams p)
        {
            var viewLoadedParams = p;

            List<NodeView> _nodeViews = viewLoadedParams.DynamoWindow.FindVisualChildren<NodeView>().ToList();
            breakTheWheel = _nodeViews;


            // THIS IS WORKING


            //var nodeView = _nodeViews.First();

            //Application.Current.Dispatcher.BeginInvoke(new Action(() => { ((Rectangle)nodeView.grid.FindName("nodeBackground")).Fill = new SolidColorBrush(Colors.Red); }));
            
        }

        public static List<NodeView> TrainWreck() {
            return breakTheWheel;
        }


        private string dynamoReferenceFilePath = "";
        public string DynamoReferenceFilePath //use this as the binding for the reference Dynamo graph
        {
            get { return dynamoReferenceFilePath; }
            set
            {
                dynamoReferenceFilePath = value;
                //RaisePropertyChanged("DynamoReferenceFilePath"); 
            }
        }




        public void Dispose()
        {

        }



        //private string activeNodeTypes;
        //private ReadyParams readyParams;


        //// Displays active nodes in the workspace
        //public string ActiveNodeTypes
        //{
        //    get
        //    {
        //        activeNodeTypes = getNodeTypes();
        //        return activeNodeTypes;
        //    }
        //}

        //// Helper function that builds string of active nodes
        //public string getNodeTypes()
        //{
        //    string output = "Active nodes:\n";

        //    foreach (NodeModel node in readyParams.CurrentWorkspaceModel.Nodes)
        //    {
        //        string nickName = node.Name;
        //        output += nickName + "\n";
        //    }

        //    return output;
        //}



        //public TrackWindowViewModel(ReadyParams p)
        //{
        //    readyParams = p;
        //    p.CurrentWorkspaceModel.NodeAdded += CurrentWorkspaceModel_NodesChanged;
        //    p.CurrentWorkspaceModel.NodeRemoved += CurrentWorkspaceModel_NodesChanged;
        //}

        //private void CurrentWorkspaceModel_NodesChanged(NodeModel obj)
        //{
        //    RaisePropertyChanged("ActiveNodeTypes");
        //}

        //public void Dispose()
        //{
        //    //readyParams.CurrentWorkspaceModel.NodeAdded -= CurrentWorkspaceModel_NodesChanged;
        //    //readyParams.CurrentWorkspaceModel.NodeRemoved -= CurrentWorkspaceModel_NodesChanged;
        //}
    }
}
