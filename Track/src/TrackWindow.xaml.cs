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
            //test location for Michael C:\Users\vantelgm7702\OneDrive - ARCADIS\Michael\Dynamo\CLO vloertje.dyn

            //2) Unlock checkboxes, set text to grey and grey out the textbox
            if (FileExists && ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                //A file woud found
                FilePathBox.IsEnabled = false;
                ButtonLoadDispose.Content = "Dispose of current reference graph";
                CheckBox_ShowAddedNodes.IsEnabled = true;
                CheckBox_ShowDeletedNodes.IsEnabled = true;
                CheckBox_CompareModifiedNodes.IsEnabled = true;
                CheckBox_ShowAddedWires.IsEnabled = true;
                CheckBox_ShowDeletedWires.IsEnabled = true;

                MessageBox.Show("File exists, ready to compare graphs");

                functions.CompareSomeGraphs(viewLoadedParams);
            }
            else if (ButtonLoadDispose.Content.ToString() == "Lock and load reference graph")
            {
                MessageBox.Show("File was not found, please try again");
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

                MessageBox.Show("File unloaded");
            }

            //MessageBox.Show("The Dynamo location is: " + (MainGrid.DataContext as TrackWindowViewModel).DynamoReferenceFilePath );
        }
    }
}
