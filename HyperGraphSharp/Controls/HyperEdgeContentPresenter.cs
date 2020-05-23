using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HyperGraphSharp.Controls
{
    public class HyperEdgeContentPresenter : ContentPresenter
    {
        public HyperEdgeContentPresenter()
        {
            LayoutUpdated += EdgeContentPresenter_LayoutUpdated;
        }

        private static HyperEdgeControl? GetHyperEdgeControl(DependencyObject parent)
        {
            while (parent != null)
                if (parent is HyperEdgeControl control)
                    return control;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            return null;
        }

        private static double GetLabelDistance(double edgeLength)
        {
            return edgeLength / 2;
        }

        private void EdgeContentPresenter_LayoutUpdated(object? sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            var hyperEdgeControl = GetHyperEdgeControl(VisualParent);
            if (hyperEdgeControl == null)
                return;

            var p = new Point
            {
                X = hyperEdgeControl.Vertices.Average(HyperGraphCanvas.GetX),
                Y = hyperEdgeControl.Vertices.Average(HyperGraphCanvas.GetY)
            };
            Arrange(new Rect(p, DesiredSize));
        }
    }
}