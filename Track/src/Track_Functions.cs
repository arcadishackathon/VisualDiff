using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows;

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

        public void CompareSomeGraphs()
        { 
            //Add Laurence's stuff here to generate the lists

            //Only proceed if there is 
            if (ReferenceFileExists)
            {

            }
        }

        public void ToggleAddedNodes()
        {
            //Code for comparing added nodes here
        }

    }
}
