using System;
using System.Windows.Media;
using System.Windows.Shapes;
using Dynamo.Controls;
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

    }
}
