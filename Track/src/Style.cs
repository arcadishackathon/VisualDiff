using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph;
using Dynamo.Models;
using Dynamo.UI.Commands;
using CoreNodeModels;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Nodes;
using static Dynamo.Models.DynamoModel;
using Dynamo.Controls;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Controls;
//using System.Drawing;

namespace Track
{
    public class Style
    {
        public NodeView nodeView;

        public Style(NodeView n)
        {
            nodeView = n;
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
            ((Rectangle)nodeView.grid.FindName("nodeBackground")).Fill = RGBA(r, g, b, a);
        }

        public void nodeBorder(int r, int g, int b, int a = 255)
        {
            ((Rectangle)nodeView.grid.FindName("nodeBorder")).Stroke = RGBA(r, g, b, a);
        }

        public void nameBackground(int r, int g, int b, int a = 255)
        {
            ((Rectangle)nodeView.grid.FindName("NameBackground")).Fill = RGBA(r, g, b, a);
        }

        public void nameBorder(int r, int g, int b, int a = 255)
        {
            ((Rectangle)nodeView.grid.FindName("NameBackground")).Stroke = RGBA(r, g, b, a);
        }

        public void portBackground(int r, int g, int b, int a = 255)
        {
            ItemsControl ports = (ItemsControl)nodeView.grid.FindName("inputPortControl");

            if (ports.Items.Count > 0)
            {
                var rectangles = ports.FindVisualChildren<Rectangle>().ToList();
                foreach (Rectangle rectangle in rectangles)
                {
                    ((Rectangle)rectangle.FindName("highlightOverlay")).Fill = RGBA(r, g, b, a);
                    ((Rectangle)rectangle.FindName("highlightOverlayForArrow")).Fill = RGBA(r, g, b, a);
                }

            }
        }

    }
}
