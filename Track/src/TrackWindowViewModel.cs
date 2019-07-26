using System.Windows;
using System;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;

namespace Track
{
    public class TrackWindowViewModel : NotificationObject, IDisposable
    {

        public TrackWindowViewModel()
        {
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


        //non-deleted stuff from the original viewextension, could be useful in the future


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
