using System.Windows;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;

namespace Track
{
    /// <summary>
    /// Interaction logic for TrackWindow.xaml
    /// </summary>
    public partial class TrackWindow : Window 
    {
        /// <summary>
        /// Reference to the current Dynamo ViewLoadedParams
        /// </summary>
        ViewLoadedParams ViewLoadedParams;

        /// <summary>
        /// Reference to Track_Functions so all its methods can be triggered
        /// </summary>
        src.Track_Functions Trigger;

        public TrackWindow(ViewLoadedParams vlp)
        {

            //Store the ViewLoadedParams as a field so it can be used in other methods
            ViewLoadedParams = vlp;

            // Load the XAML (I think..)
            InitializeComponent();

            // Include a reference to the Track_Functions
            Trigger = new src.Track_Functions();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //don't need this but keep for reference
            //(MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath = FilePathBox.Text;

            string referenceFilePath = FilePathBox.Text;
            //steps:
            //1) invoke a method to check validity of the Reference graph location
            //2) Unlock the checkboxes
            //3) grey out the textbox

            //1) check if the location if OK
            bool FileExists = Trigger.CheckReferenceFileIsValid(referenceFilePath);

            //2) Unlock checkboxes, set text to grey and grey out the textbox

            // If a file was selected and it is valid
            if (FileExists && ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                FilePathBox.IsEnabled = false;
                ButtonLoadDispose.Content = "Dispose of current reference graph";

                // Enable the checkboxes
                CheckBox_ShowAddedNodes.IsEnabled = true;
                CheckBox_ShowDeletedNodes.IsEnabled = true;
                CheckBox_CompareModifiedNodes.IsEnabled = true;
                CheckBox_ShowAddedWires.IsEnabled = true;
                CheckBox_ShowDeletedWires.IsEnabled = true;

                // Set checkbox defaults
                CheckBox_ShowAddedNodes.IsChecked = true;
                CheckBox_ShowDeletedNodes.IsChecked = true;

                //start the comparison using the referenceFilePath
                Trigger.CompareSomeGraphs(ViewLoadedParams, referenceFilePath);

                // Toggle the ADDED and DELETED on by default.
                Trigger.ShowDeletedNodes();
                Trigger.HighlightAddedNodes();

                MessageBox.Show("File exists, ready to compare graphs", "Reference Dynamo graph",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            }
            // If a file was selected but it wasn't valid.
            else if (ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                MessageBox.Show("File was not found, please try again", "Reference Dynamo graph",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Else file has been unloaded
            else
            {
                FilePathBox.IsEnabled = true;
                ButtonLoadDispose.Content = "Lock and load reference graph";
                FilePathBox.Text = "";

                // Disable the checkboxes
                CheckBox_ShowAddedNodes.IsEnabled = false;
                CheckBox_ShowDeletedNodes.IsEnabled = false;
                CheckBox_CompareModifiedNodes.IsEnabled = false;
                CheckBox_ShowAddedWires.IsEnabled = false;
                CheckBox_ShowDeletedWires.IsEnabled = false;

                // Untick all checkboxes
                //@todo Why are we doing this? Shouldn't we define the default values here?
                CheckBox_ShowAddedNodes.IsChecked = false;
                CheckBox_ShowDeletedNodes.IsChecked = false;
                CheckBox_CompareModifiedNodes.IsChecked = false;
                CheckBox_ShowAddedWires.IsChecked = false;
                CheckBox_ShowDeletedWires.IsChecked = false;

                UnloadAllChanges();

                MessageBox.Show("File unloaded", "Reference Dynamo graph",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            }

            //MessageBox.Show("The Dynamo location is: " + (MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath );
        }

        //checkbox functionalty
        private void CheckBox_ShowDeletedNodes_Checked(object sender, RoutedEventArgs e)
        {
            Trigger.ShowDeletedNodes();
        }

        private void CheckBox_ShowDeletedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            Trigger.RemoveDeletedNodes();
        }

        private void CheckBox_ShowAddedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            Trigger.UnhighlightAddedNodes();

        }

        private void CheckBox_ShowAddedNodes_Checked(object sender, RoutedEventArgs e)
        {
            Trigger.HighlightAddedNodes();

        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            UnloadAllChanges();
        }

        private void UnloadAllChanges()
        {
            //Disable all active states to return to the current graph
            Trigger.UnhighlightAddedNodes();
            Trigger.RemoveDeletedNodes();
        }
    }
}
