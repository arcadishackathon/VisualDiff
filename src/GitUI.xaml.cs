using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using System.IO;

namespace Track
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class GitUI : Window 
    {
        /// <summary>
        /// Reference to the current Dynamo ViewLoadedParams
        /// </summary>
        ViewLoadedParams ViewLoadedParams;

        /// <summary>
        /// Reference to Compare so all its methods can be triggered
        /// </summary>
        Compare Compare;

        //public Dictionary<string, string> Commits;

        bool ReferenceFileLoaded = false;

        public GitUI(ViewLoadedParams vlp)
        {
            //Store the ViewLoadedParams as a field so it can be used in other methods
            ViewLoadedParams = vlp;

            // Load the XAML (I think..)
            InitializeComponent();
        }

        private void ToggleFileSelection(bool value)
        {
            TextBox_FilePath.IsEnabled = value;
            Button_SelectFile.IsEnabled = value;
        }

        private void ToggleEnabledCheckboxes(bool value)
        {
            CheckBox_ShowAddedNodes.IsEnabled = value;
            CheckBox_ShowDeletedNodes.IsEnabled = value;
        }

        private void ToggleCheckedCheckboxes(bool value)
        {
            CheckBox_ShowAddedNodes.IsChecked = value;
            CheckBox_ShowDeletedNodes.IsChecked = value;
        }

        private void SetCheckboxDefaults()
        {
            CheckBox_ShowAddedNodes.IsChecked = true;
            CheckBox_ShowDeletedNodes.IsChecked = true;
        }

        private void LoadReferenceGraph(string referenceFilePath)
        {
            // Disable the reference file path box
            ToggleFileSelection(false);

            // Change the button value
            Button_LoadDispose.Content = "Unload reference graph";

            // Enable the checkboxes
            ToggleEnabledCheckboxes(true);

            // Start a new comparison using the referenceFilePath
            Compare = new Compare(ViewLoadedParams, referenceFilePath);

            // Set checkbox defaults
            SetCheckboxDefaults();

            // Trigger ADDED and DELETED to match the CheckboxDefaults
            Compare.ShowDeletedNodes();
            Compare.HighlightAddedNodes();
        }

        private void UnloadReferenceGraph() {
            ToggleFileSelection(true);
            Button_LoadDispose.Content = "Load reference graph";

            // Disable the checkboxes
            ToggleEnabledCheckboxes(false);

            // Untick all checkboxes
            //@todo Why are we doing this? Shouldn't we define the default values here?
            ToggleCheckedCheckboxes(false);

            UnloadAllChanges();

            System.Windows.MessageBox.Show("File unloaded", "Reference Dynamo graph",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_LoadDispose_Click(object sender, RoutedEventArgs e)
        {

            // What we do on the button being clicked depends on the context.
            // If no reference file is loaded then try to load it
            if (ReferenceFileLoaded == false)
            {
                //don't need this but keep for reference
                //(MainGrid.DataContext as UIViewModel).DynamoReferenceFilePath = FilePathBox.Text;

                string referenceFilePath = TextBox_FilePath.Text;

                // Check the file is valid
                (bool isValid, string message) = Utilities.CheckReferenceFileIsValid(referenceFilePath, ViewLoadedParams, false);

                if(isValid)
                {
                    // Start a new instance of our git interface with the reference file path
                    Git git = new Git(referenceFilePath);
                    //(MainGrid.DataContext as UIViewModel).Commits = git.Log();

                    // Get all the commits that include our file
                    var commits = git.Log();

                    // Add the commits to the ComboBox selection
                    ComboBox_Commits.ItemsSource = new BindingSource(commits, null);
                    ComboBox_Commits.DisplayMemberPath = "Value";
                    ComboBox_Commits.SelectedValuePath = "Key";
                    //ComboBox_Commits.SelectedIndex = 0;

                    // Set ReferenceFileLoaded so we know to unload it on next button press
                    ReferenceFileLoaded = true;
                }
                else
                {
                    System.Windows.MessageBox.Show(message, "Reference Dynamo graph",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            // If a reference file is already loaded then unloaded it
            else
            {
                //UnloadReferenceGraph();

                // Reset ReferenceFileLoaded so we know to try to load it on next button press
                ReferenceFileLoaded = false;
            }

            //MessageBox.Show("The Dynamo location is: " + (MainGrid.DataContext as UIViewModel).DynamoReferenceFilePath );
        }
        private void Button_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog();
            fileOpen.Filter = "Dynamo Graph|*.dyn";
            DialogResult dialog = fileOpen.ShowDialog();
            if (dialog == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            TextBox_FilePath.Text = fileOpen.FileName;
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

        private void ComboBox_Commits_Selected(object sender, RoutedEventArgs e)
        {

            DynamoViewModel viewModel = ViewLoadedParams.DynamoWindow.DataContext as DynamoViewModel;
            string referenceFilePath = TextBox_FilePath.Text;

            // Start a new instance of our git interface with the reference file path
            Git git = new Git(referenceFilePath);

            string commithash = ComboBox_Commits.SelectedValue.ToString();

            // Checkout the requested commit..
            git.Checkout(commithash, true);

            // Make a TEMPORARY copy of the checked out version reference graph
            string tempReferenceFile = Path.GetTempFileName();
            File.Copy(referenceFilePath, tempReferenceFile, true);
            Debug.WriteLine(tempReferenceFile);

            // Reset staged changes for the file (from when we checked it out)
            git.Reset("HEAD", true);

            // Now discard the changes for the file
            git.Checkout("", true);

            // Load the referenceFilePath so it becomes the currentFilePath
            Utilities.LoadGraph(referenceFilePath, viewModel);

            // Now load the tempReferenceFile as the ReferenceGraph because it contains the requested commit version
            LoadReferenceGraph(tempReferenceFile);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UnloadAllChanges();
        }

        private void UnloadAllChanges()
        {
            if (ReferenceFileLoaded)
            {
                //Disable all active states to return to the current graph
                Compare.UnhighlightAddedNodes();
                Compare.RemoveDeletedNodes();

                // Reload the current graph
                Utilities.LoadGraph(Compare.CurrentGraphFileName, Compare.ViewModel);
            }
        }
    }
}
