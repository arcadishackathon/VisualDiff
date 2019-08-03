using System;
using Dynamo.Core;

namespace Track
{
    public class UIViewModel : NotificationObject, IDisposable
    {

        public UIViewModel()
        {
        }


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
