using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Core;

namespace Track
{
    public class UIViewModel : NotificationObject, IDisposable
    {

        public UIViewModel()
        {
        }

        public Dictionary<string, string> Commits { get; set; }


        private string dynamoReferenceFilePath = "";
        public string DynamoReferenceFilePath //use this as the binding for the reference Dynamo graph
        {
            get { return dynamoReferenceFilePath; }
            set
            {
                dynamoReferenceFilePath = value;
            }
        }

        public void Dispose()
        {
        }
    }
}
