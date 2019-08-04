using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dynamo.ViewModels;
using Dynamo.Wpf.Extensions;

namespace Track
{
    /// <summary>
    /// The View Extension framework for Dynamo allows you to extend
    /// the Dynamo UI by registering custom MenuItems. A ViewExtension has 
    /// two components, an assembly containing a class that implements
    /// IViewExtension, and an ViewExtensionDefintion xml file used to
    /// instruct Dynamo where to find the class containing the
    /// IViewExtension implementation. The ViewExtensionDefinition xml file must
    /// be located in your [dynamo]\viewExtensions folder.
    ///
    /// This sample demonstrates an IViewExtension implementation which
    /// shows a modeless window when its MenuItem is clicked.
    /// The Window created tracks the number of nodes in the current workspace,
    /// by handling the workspace's NodeAdded and NodeRemoved events.
    /// </summary>
    public class TrackViewExtension : IViewExtension
    {
        private MenuItem UIMenuItem;
        private MenuItem GitUIMenuItem;

        private ViewLoadedParams ViewLoadedParams;

        private DynamoViewModel DynamoViewModel => ViewLoadedParams.DynamoWindow.DataContext as DynamoViewModel;

        public void Dispose()
        {
        }

        public void Startup(ViewStartupParams vsp)
        {
        }

        public void Loaded(ViewLoadedParams vlp)
        {

            // Hold a reference to the Dynamo params to be used later
            ViewLoadedParams = vlp;

            // Add a separator to the View menu
            ViewLoadedParams.AddSeparator(MenuBarType.View, new Separator());

            // Create a new menu item
            UIMenuItem = new MenuItem { Header = "Visually compare the current graph with a reference graph" };

            // Make it stand out with a colour
            UIMenuItem.Foreground = new SolidColorBrush(Color.FromArgb(
                Convert.ToByte(255),
                Convert.ToByte(255),
                Convert.ToByte(255),
                Convert.ToByte(255)
            ));

            UIMenuItem.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/Track;component/Resources/arcadis-icon.png", UriKind.Relative))
            };

            // Start our extension on click
            UIMenuItem.Click += (sender, args) =>
            {
                // Load the Extension ViewModel
                var viewModel = new UIViewModel();

                // Load the Window
                // This is where the magic begins!
                var window = new UI(ViewLoadedParams)
                {
                    // Set the data context for the main grid in the window.
                    MainGrid = { DataContext = viewModel },

                    // Set the owner of the window to the Dynamo window.
                    Owner = ViewLoadedParams.DynamoWindow
                };

                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                // Show a modeless window.
                window.Show();
            };

            // Add the menu item to the View menu
            ViewLoadedParams.AddMenuItem(MenuBarType.View, UIMenuItem);

            // Create a new menu item
            GitUIMenuItem = new MenuItem { Header = "Compare with Git" };

            // Make it stand out with a colour
            GitUIMenuItem.Foreground = new SolidColorBrush(Color.FromArgb(
                Convert.ToByte(255),
                Convert.ToByte(255),
                Convert.ToByte(255),
                Convert.ToByte(255)
            ));

            GitUIMenuItem.Icon = new Image
            {
                Source = new BitmapImage(new Uri("/Track;component/Resources/arcadis-icon.png", UriKind.Relative))
            };

            // Start our extension on click
            GitUIMenuItem.Click += (sender, args) =>
            {
                // Load the Extension ViewModel
                var viewModel = new UIViewModel();

                // Load the Window
                // This is where the magic begins!
                var window = new GitUI(ViewLoadedParams)
                {
                    // Set the data context for the main grid in the window.
                    MainGrid = { DataContext = viewModel },

                    // Set the owner of the window to the Dynamo window.
                    Owner = ViewLoadedParams.DynamoWindow
                };

                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                // Show a modeless window.
                window.Show();
            };

            // Add the menu item to the View menu
            ViewLoadedParams.AddMenuItem(MenuBarType.View, GitUIMenuItem);
        }

        public void Shutdown()
        {
        }

        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }

        public string Name
        {
            get
            {
                return "Track View Extension";
            }
        }

    }
}