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

        private void ToggleEnabledCheckboxes(bool value)
        {
            CheckBox_ShowAddedNodes.IsEnabled = value;
            CheckBox_ShowDeletedNodes.IsEnabled = value;
            CheckBox_CompareModifiedNodes.IsEnabled = value;
            CheckBox_ShowAddedWires.IsEnabled = value;
            CheckBox_ShowDeletedWires.IsEnabled = value;
        }

        private void ToggleCheckedCheckboxes(bool value)
        {
            CheckBox_ShowAddedNodes.IsChecked = value;
            CheckBox_ShowDeletedNodes.IsChecked = value;
            CheckBox_CompareModifiedNodes.IsChecked = value;
            CheckBox_ShowAddedWires.IsChecked = value;
            CheckBox_ShowDeletedWires.IsChecked = value;
        }

        private void SetCheckboxDefaults()
        {
            CheckBox_ShowAddedNodes.IsChecked = true;
            CheckBox_ShowDeletedNodes.IsChecked = true;
            //CheckBox_CompareModifiedNodes.IsChecked = false;
            //CheckBox_ShowAddedWires.IsChecked = false;
            //CheckBox_ShowDeletedWires.IsChecked = false;
        }

        private void LoadReferenceGraph(string referenceFilePath)
        {
            // Disable the reference file path box
            FilePathBox.IsEnabled = false;

            // Change the button value
            ButtonLoadDispose.Content = "Dispose of current reference graph";

            // Enable the checkboxes
            ToggleEnabledCheckboxes(true);

            // Set checkbox defaults
            SetCheckboxDefaults();

            //start the comparison using the referenceFilePath
            Trigger.CompareSomeGraphs(ViewLoadedParams, referenceFilePath);

            // Trigger ADDED and DELETED to match the CheckboxDefaults
            Trigger.ShowDeletedNodes();
            Trigger.HighlightAddedNodes();

            MessageBox.Show("File exists, ready to compare graphs", "Reference Dynamo graph",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UnloadReferenceGraph() {
            FilePathBox.IsEnabled = true;
            ButtonLoadDispose.Content = "Lock and load reference graph";
            FilePathBox.Text = "";

            // Disable the checkboxes
            ToggleEnabledCheckboxes(false);

            // Untick all checkboxes
            //@todo Why are we doing this? Shouldn't we define the default values here?
            ToggleCheckedCheckboxes(false);

            UnloadAllChanges();

            MessageBox.Show("File unloaded", "Reference Dynamo graph",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
                LoadReferenceGraph(referenceFilePath);
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
                UnloadReferenceGraph();

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
