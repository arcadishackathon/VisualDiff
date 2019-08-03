using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Track
{
    public static class Utilies
    {
        public static Tuple<bool, string> CheckReferenceFileIsValid(string filePath)
        {
            // Assume it's true by default
            // We'll change this later if it's not valid
            bool valid = true;
            string message = "File is valid";

            // Does the file exist
            if (!File.Exists(filePath))
            {
                valid = false;
                message = "File not found";
            }
            // Is is it a .dyn file?
            else if (Path.GetExtension(filePath) != ".dyn")
            {
                valid = false;
                message = "File is not a Dynamo Graph";
            }
            return Tuple.Create(valid, message);
        }
    }
}
