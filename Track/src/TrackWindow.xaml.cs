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



        private ViewLoadedParams viewLoadedParams;

        //Fields
        src.Track_Functions functions;

        public TrackWindow(ViewLoadedParams p)
        {
            viewLoadedParams = p;

            InitializeComponent();

            functions = new src.Track_Functions();
            //DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //don't need this but keep for reference
            //(MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath = FilePathBox.Text;

            string FilePath = FilePathBox.Text;
            //steps:
            //1) invoke a method to check validity of the Reference graph location
            //2) Unlock the checkboxes
            //3) grey out the textbox

            //1) check if the location if OK
            bool FileExists = functions.CheckReferenceDynamoGraphFileLocationValidity(FilePath);
            //test location for Michael
            //C:\Users\vantelgm7702\OneDrive - ARCADIS\Michael\Computation\35. Toronto Hackaton\01. Dynamo files\Version 1.dyn

            //2) Unlock checkboxes, set text to grey and grey out the textbox
            if (FileExists && ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                //A file was found
                FilePathBox.IsEnabled = false;
                ButtonLoadDispose.Content = "Dispose of current reference graph";
                CheckBox_ShowAddedNodes.IsEnabled = true;
                CheckBox_ShowDeletedNodes.IsEnabled = true;
                CheckBox_CompareModifiedNodes.IsEnabled = true;
                CheckBox_ShowAddedWires.IsEnabled = true;
                CheckBox_ShowDeletedWires.IsEnabled = true;

                CheckBox_ShowAddedNodes.IsChecked = true;
                CheckBox_ShowDeletedNodes.IsChecked = true;

                //start the comparison using the filelocation
                functions.CompareSomeGraphs(viewLoadedParams, FilePath);
                functions.ToggleRemovedNodes(true);
                functions.ToggleAddedNodes(true);

                MessageBox.Show("File exists, ready to compare graphs", "Reference Dynamo graph",
                    MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                MessageBox.Show("File was not found, please try again", "Reference Dynamo graph",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                FilePathBox.IsEnabled = true;
                ButtonLoadDispose.Content = "Lock and load reference graph";
                FilePathBox.Text = "";
                CheckBox_ShowAddedNodes.IsEnabled = false;
                CheckBox_ShowDeletedNodes.IsEnabled = false;
                CheckBox_CompareModifiedNodes.IsEnabled = false;
                CheckBox_ShowAddedWires.IsEnabled = false;
                CheckBox_ShowDeletedWires.IsEnabled = false;

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
            bool checkbox = true;
            functions.ToggleRemovedNodes(checkbox);
        }
        private void CheckBox_ShowDeletedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            bool checkbox = false;
            functions.ToggleRemovedNodes(checkbox);
        }

        private void CheckBox_ShowAddedNodes_Unchecked(object sender, RoutedEventArgs e)
        {
            bool checkbox = false;
            functions.ToggleAddedNodes(checkbox);

        }
        private void CheckBox_ShowAddedNodes_Checked(object sender, RoutedEventArgs e)
        {
            bool checkbox = true;
            functions.ToggleAddedNodes(checkbox);

        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            UnloadAllChanges();
        }
        private void UnloadAllChanges()
        {
            //Disable all active states to return to the current graph
            functions.ToggleAddedNodes(false);
            functions.ToggleRemovedNodes(false);
        }
    }
}
