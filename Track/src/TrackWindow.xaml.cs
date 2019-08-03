using System;
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
        /// Reference to Compare so all its methods can be triggered
        /// </summary>
        Compare Compare;

        bool ReferenceFileLoaded = false;

        public TrackWindow(ViewLoadedParams vlp)
        {
            //Store the ViewLoadedParams as a field so it can be used in other methods
            ViewLoadedParams = vlp;

            // Load the XAML (I think..)
            InitializeComponent();
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

            // Load a new instance of the Functions Class
            Compare = new Compare();

            // Set checkbox defaults
            SetCheckboxDefaults();

            //start the comparison using the referenceFilePath
            Compare.CompareSomeGraphs(ViewLoadedParams, referenceFilePath);

            // Trigger ADDED and DELETED to match the CheckboxDefaults
            Compare.ShowDeletedNodes();
            Compare.HighlightAddedNodes();

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
            // What we do on the button being clicked depends on the context.
            // If no reference file is loaded then try to load it
            if(ReferenceFileLoaded == false)
            {
                //don't need this but keep for reference
                //(MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath = FilePathBox.Text;

                string referenceFilePath = FilePathBox.Text;

                // Check the file is valid
                (bool isValid, string message) = Utilies.CheckReferenceFileIsValid(referenceFilePath);

                if(isValid)
                {
                    LoadReferenceGraph(FilePathBox.Text);

                    // Set ReferenceFileLoaded so we know to unload it on next button press
                    ReferenceFileLoaded = true;
                }
                else
                {
                    MessageBox.Show(message, "Reference Dynamo graph",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            // If a reference file is already loaded then unloaded it
            else
            {
                UnloadReferenceGraph();

                // Reset ReferenceFileLoaded so we know to try to load it on next button press
                ReferenceFileLoaded = false;
            }

            //MessageBox.Show("The Dynamo location is: " + (MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath );
        }

        //checkbox functionalty
        private void CheckBox_ShowDeletedNodes_Checked(object sender, RoutedEventArgs e)
        {
            Compare.ShowDeletedNodes();
        }

        private void CheckBox_ShowDeletedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            Compare.RemoveDeletedNodes();
        }

        private void CheckBox_ShowAddedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            Compare.UnhighlightAddedNodes();

        }

        private void CheckBox_ShowAddedNodes_Checked(object sender, RoutedEventArgs e)
        {
            Compare.HighlightAddedNodes();

        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            UnloadAllChanges();
        }

        private void UnloadAllChanges()
        {
            //Disable all active states to return to the current graph
            Compare.UnhighlightAddedNodes();
            Compare.RemoveDeletedNodes();
        }
    }
}
