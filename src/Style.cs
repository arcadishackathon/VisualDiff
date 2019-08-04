using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Controls;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Controls;
//using System.Drawing;

namespace Track
{
    public class Style
    {
        public NodeView NodeView;

        public Style(NodeView n)
        {
            NodeView = n;
        }

        public SolidColorBrush RGBA(int r, int g, int b, int a = 255)
        {
            // Note: System.Drawing.Color.FromArgb() accepts Int32. System.Windows.Media.Color.FromArgb() requires Bytes.
            return new SolidColorBrush(Color.FromArgb(
                Convert.ToByte(a),
                Convert.ToByte(r),
                Convert.ToByte(g),
                Convert.ToByte(b)
            ));
        }

        public void nodeBackground(int r, int g, int b, int a = 255)
        {
            ((Rectangle)NodeView.grid.FindName("nodeBackground")).Fill = RGBA(r, g, b, a);
        }

        public void nodeBorder(int r, int g, int b, int a = 255)
        {
            ((Rectangle)NodeView.grid.FindName("nodeBorder")).Stroke = RGBA(r, g, b, a);
        }

        public void nameBackground(int r, int g, int b, int a = 255)
        {
            ((Rectangle)NodeView.grid.FindName("NameBackground")).Fill = RGBA(r, g, b, a);
        }

        public void nameBorder(int r, int g, int b, int a = 255)
        {
            ((Rectangle)NodeView.grid.FindName("NameBackground")).Stroke = RGBA(r, g, b, a);
        }

        public void portBackground(int r, int g, int b, int a = 255)
        {
            ItemsControl inputs = (ItemsControl)NodeView.grid.FindName("inputPortControl");

            if (inputs.Items.Count > 0)
            {
                var rectangles = inputs.FindVisualChildren<Rectangle>().ToList();
                foreach (Rectangle rectangle in rectangles)
                {
                    try
                    {
                        // @todo One of my test scripts is throwing an exception when trying to access the fill of a port that doesn't exist.
                        // I thought if (inputs.Items.Count > 0) would handle this but apparently not.. Therefore lets catch and ignore the exception.
                        ((Rectangle)rectangle.FindName("highlightOverlay")).Fill = RGBA(r, g, b, a);
                        ((Rectangle)rectangle.FindName("highlightOverlayForArrow")).Fill = RGBA(r, g, b, a);

                    }
                    catch (System.NullReferenceException e)
                    {
                        Utilities.Debug(e, e.Message, "Could not locate input port to colour.", "Continuing without colouring the input port...");
                    }
                    
                }

            }

            ItemsControl outputs = (ItemsControl)NodeView.grid.FindName("outputPortControl");

            if (outputs.Items.Count > 0)
            {
                var rectangles = outputs.FindVisualChildren<Rectangle>().ToList();
                foreach (Rectangle rectangle in rectangles)
                {
                    try
                    {
                        ((Rectangle)rectangle.FindName("highlightOverlay")).Fill = RGBA(r, g, b, a);
                        ((Rectangle)rectangle.FindName("highlightOverlayForArrow")).Fill = RGBA(r, g, b, a);

                    }
                    catch (System.NullReferenceException e)
                    {
                        Utilities.Debug(e, e.Message, "Could not locate output port to colour.", "Continuing without colouring the output port...");
                    }
                }

            }
        }

    }
}
