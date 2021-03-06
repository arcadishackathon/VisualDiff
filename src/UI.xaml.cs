﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;

namespace Track
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : Window 
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

        public UI(ViewLoadedParams vlp)
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

        private void SetReferenceFileLoaded(bool value)
        {
            ReferenceFileLoaded = value;
            if (value)
            {
                Button_Load.Visibility = Visibility.Hidden;
                Button_Dispose.Visibility = Visibility.Visible;
            }
            else
            {
                Button_Load.Visibility = Visibility.Visible;
                Button_Dispose.Visibility = Visibility.Hidden;
            }
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

            SetReferenceFileLoaded(true);

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

            // Disable the checkboxes
            ToggleEnabledCheckboxes(false);

            // Untick all checkboxes
            //@todo Why are we doing this? Shouldn't we define the default values here?
            ToggleCheckedCheckboxes(false);

            UnloadAllChanges();

            System.Windows.MessageBox.Show("File unloaded", "Reference Dynamo graph",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            //don't need this but keep for reference
            //(MainGrid.DataContext as UIViewModel).DynamoReferenceFilePath = FilePathBox.Text;

            string referenceFilePath = TextBox_FilePath.Text;

            // Check the file is valid
            (bool isValid, string message) = Utilities.CheckReferenceFileIsValid(referenceFilePath, ViewLoadedParams);

            if(isValid)
            {
                LoadReferenceGraph(TextBox_FilePath.Text);
            }
            else
            {
                System.Windows.MessageBox.Show(message, "Reference Dynamo graph",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //MessageBox.Show("The Dynamo location is: " + (MainGrid.DataContext as UIViewModel).DynamoReferenceFilePath );
        }

        private void Button_Dispose_Click(object sender, RoutedEventArgs e)
        {
            UnloadReferenceGraph();
        }

        private void Button_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog();

            // Join the file types like this: *.dyn;*.dyf
            string fileTypes = string.Join(";", Utilities.DynamoFileExtensions.Select(x => "*" + x));

            // Create type filter
            fileOpen.Filter = "Dynamo Graph (" + fileTypes + ")|" + fileTypes;

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

                SetReferenceFileLoaded(false);

                // Reload the current graph
                Utilities.LoadGraph(Compare.CurrentGraphFileName, Compare.ViewModel);
            }
        }
    }
}
